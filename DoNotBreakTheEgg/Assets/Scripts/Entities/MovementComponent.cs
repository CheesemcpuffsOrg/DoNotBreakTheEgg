using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementComponent : MonoBehaviour, IMovementComponent
{
    private struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle;
        public float oldSlopeAngle;

        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            right = left = false;
            climbingSlope = false;
            descendingSlope = false;

            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 topleft;
        public Vector2 topright;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    [SerializeField] float jumpHeight = 4;
    [SerializeField] float timeToJumpApex = 0.4f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float maxClimbAngle = 80;
    [SerializeField] float maxDescendAngle = 80;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;
    

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float VelocityXSmoothing;

    float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isGroundedTag;
    [SerializeField] TagFilter jumpFilter;
    [SerializeField] TagFilter moveFilter;
    //is grounded filter

    [Header("Raycasting")]
    [SerializeField] int horizontalRayCount = 4;
    [SerializeField] int verticalRayCount = 4;
    [SerializeField] LayerMask collisionMask;

    const float skinWidth = 0.015f;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    

    RaycastOrigins raycastOrigins;
    CollisionInfo collisionInfo;

    Vector2 input;
    bool jumpFired;

    IEntity entity;
    ICollisionComponent collisionComponent;
    ITagComponent tagComponent;
    IGameObjectComponent gameObjectComponent;

    private void Start()
    {
        entity = GetComponent<IEntity>();
        collisionComponent = entity.GetEntityComponent<ICollisionComponent>();
        tagComponent = entity.GetEntityComponent<ITagComponent>();
        gameObjectComponent = entity.GetEntityComponent<IGameObjectComponent>();

        CalculateRaySpacing();

        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {

        CheckIfGrounded();

        if (collisionInfo.above || collisionInfo.below)
        {
            velocity.y = 0;
        }

        ProcessJump();

        var targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref VelocityXSmoothing, (collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        Move(velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (!jumpFilter.PassTagFilterCheck(gameObjectComponent.GetTransform())) return;

        jumpFired = true;
        jumpBufferCounter = jumpBufferTime;
    }

    public void SetTarget(Vector2 target)
    {
        if (!moveFilter.PassTagFilterCheck(gameObjectComponent.GetTransform())) return;

        input = target;
    }

    public void StopMovement()
    {
        input = Vector2.zero;
    }

    private void CheckIfGrounded()
    {
        if (collisionInfo.below)
        {
            tagComponent.AddTag(isGroundedTag);
        }
        else
        {
            tagComponent.RemoveTag(isGroundedTag);
        }
    }

    private void ProcessJump()
    {
        if (jumpFired)
        {
            if (collisionInfo.below)
            {
                velocity.y = jumpVelocity;
                jumpFired = false;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
                if (jumpBufferCounter < 0)
                {
                    jumpFired = false;
                }
            }
        }
    }

    void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.velocityOld = velocity;

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
        Physics2D.SyncTransforms(); //sync all child objects with parent object
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        var directionX = Mathf.Sign(velocity.x);
        var rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            foreach(var hit in hits)
            {
                //ignore colliders if they are entities own colliders
                if (collisionComponent.IsEntityCollider(hit.collider))
                {
                    continue;
                }

                // Check if the raycast hit something
                if (hit)
                {
                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= maxClimbAngle)
                    {

                        if (collisionInfo.descendingSlope)
                        {
                            collisionInfo.descendingSlope = false;
                            velocity = collisionInfo.velocityOld;
                        }

                        var distanceToSlopeStart = 0f;

                        if(slopeAngle != collisionInfo.oldSlopeAngle)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            velocity.x -= distanceToSlopeStart * directionX;
                        }

                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    if(!collisionInfo.climbingSlope || slopeAngle > maxClimbAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisionInfo.climbingSlope)
                        {
                            velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }

                        collisionInfo.left = directionX == -1;
                        collisionInfo.right = directionX == 1;
                    }   
                }
            }   
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        var directionY = Mathf.Sign(velocity.y);
        var rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            var rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topleft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            foreach (var hit in hits)
            {
                //ignore colliders if they are entities own colliders
                if (collisionComponent.IsEntityCollider(hit.collider))
                {
                    continue;
                }
                
                // Process the valid hit
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;

                break;
            }
        }

        if (collisionInfo.climbingSlope)
        {
            var directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            var rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            foreach (var hit in hits) 
            {

                //ignore colliders if they are entities own colliders
                if (collisionComponent.IsEntityCollider(hit.collider))
                {
                    continue;
                }

                if (hit)
                {
                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != collisionInfo.slopeAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        collisionInfo.slopeAngle = slopeAngle;

                        break;
                    }
                }
            }

            
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        var moveDistance = Mathf.Abs(velocity.x);
        var climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y > climbVelocityY) return;
        
        velocity.y = climbVelocityY;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        collisionInfo.below = true;
        collisionInfo.climbingSlope = true;
        collisionInfo.slopeAngle = slopeAngle;
    }

    void DescendSlope(ref Vector3 velocity)
    {
        var directionX = Mathf.Sign(velocity.x);

        var rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        var hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        foreach (var hit in hits)
        {
            //ignore colliders if they are entities own colliders
            if (collisionComponent.IsEntityCollider(hit.collider))
            {
                continue;
            }

            if (hit)
            {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            var moveDistance = Mathf.Abs(velocity.x);
                            var descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.below = true;

                            break;
                        }
                    }
                }
            }
        }

            
    }

    void CalculateRaySpacing()
    {
        var bounds = collisionComponent.GetMainPlayerColliderBounds();
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins()
    {
        var bounds = collisionComponent.GetMainPlayerColliderBounds();
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topleft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topright = new Vector2(bounds.max.x, bounds.max.y);
    }
}

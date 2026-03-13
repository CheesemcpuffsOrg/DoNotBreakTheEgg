using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    [SerializeField] EntityDataScriptableObject data;

    float localGravity;
    bool gravityEnabled;
    bool movementEnabled;

    float jumpVelocity;

    Vector3 velocity;
    public Vector3 Velocity => velocity;
    float VelocityXSmoothing;

    float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isGroundedTag;
    [SerializeField] TagFilter jumpFilter;
    [SerializeField] TagFilter moveFilter;

    [Header("Raycasting")]
    [SerializeField] int horizontalRayCount = 4;
    [SerializeField] int verticalRayCount = 4;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] Vector2 verticalRayOffset = new Vector2(0,0); // this works for the egg but isn't actually doing what is expcted

    [SerializeField, Min(0)] float skinWidth = 0.01f;
    private readonly RaycastHit2D[] raycastBuffer = new RaycastHit2D[20]; // Tune size


    [Header("Audio")]
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData jumpSoundData;
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData jumpVocalSoundData;

   
    float horizontalRaySpacing;
    float verticalRaySpacing;

    RaycastOrigins raycastOrigins;
    CollisionInfo collisionInfo;

    Vector2 input;
    bool jumpFired;

    bool throwFired;
    int frameSkipper = 1;

    IEntity entity;
    ICollisionComponent collisionComponent;
    ITagComponent tagComponent;
    ISoundComponent soundComponent;

    #region --TEST--

    bool ignoreFilter;

    public void TestToggleIgnoreJumpFilter()
    {
        ignoreFilter = !ignoreFilter;
    }
    #endregion

    private void Start()
    {
        entity = GetComponent<IEntity>();
        collisionComponent = entity.GetEntityComponent<ICollisionComponent>();
        tagComponent = entity.GetEntityComponent<ITagComponent>();
        soundComponent = entity.GetEntityComponent<IEntitySoundComponent>();

        CalculateRaySpacing();

        localGravity = GlobalDataManager.Instance.Gravity * data.weight;
        jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(localGravity) * data.JumpHeight);

        gravityEnabled = data.GravityEnabledOnStart;
        movementEnabled = true;
    }

    private void Update()
    {
        IsCeilinged();

        ProcessJump();

        ProcessThrow();

        CalculateInputVelocity();

        Gravity();

        Move(velocity * Time.deltaTime);

        IsGrounded();

    }

    public void Jump()
    {
        if (!ignoreFilter)
        {
            //Debug.Log(entity + "" + entity.GetEntityComponent<ITagComponent>() + "" + entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(jumpFilter));
            if (!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(jumpFilter)) return;
        }


        jumpFired = true;
        jumpBufferCounter = jumpBufferTime;
    }

    public void MoveToTarget(Vector2 target)
    {
        if (!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(moveFilter)) return;

        input = target;
    }

    public void Throw(float power, Vector2 direction)
    {
        velocity = Vector2.zero; //reset the velocity to make sure no previous velocity is impacting the throw
        velocity += new Vector3(direction.x, direction.y, 0) * power;
        frameSkipper = 1;
        throwFired = true;
        EnableGravity();
    }

    public void StopMovement()
    {
        input = Vector2.zero;
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    public void DisableMovement()
    {
        movementEnabled = false;
    }

    public void EnableGravity()
    {
        gravityEnabled = true;
    }
    public void DisableGravity()
    {
        gravityEnabled = false;
        velocity.y = 0;
    }

    //might be firing more than expected
    private void IsGrounded()
    {
        if (throwFired)
            return;

        if (collisionInfo.below)
        {
            velocity.y = 0;

            if (!tagComponent.HasTag(isGroundedTag))
                tagComponent.AddTag(isGroundedTag);
        }
        else
        {
            if (tagComponent.HasTag(isGroundedTag))
                tagComponent.RemoveTag(isGroundedTag);
        }
    }

    private void IsCeilinged()
    {
        if (collisionInfo.above)
        {
            velocity.y = 0;
        }
    }

    private void CalculateInputVelocity()
    {
        if (throwFired)
            return;

        float targetVelocityX = input.x * data.MoveSpeed;

        float smoothTime;

        bool accelerating = 
            Mathf.Sign(targetVelocityX) == Mathf.Sign(velocity.x) &&
            Mathf.Abs(targetVelocityX) > Mathf.Abs(velocity.x);

        if (collisionInfo.below)
        {
            smoothTime = accelerating
                ? data.AccelerationTimeGrounded
                : data.DecelerationTimeGrounded;
        }
        else
        { 
            smoothTime = data.AccelerationTimeGrounded;
        }

        velocity.x = Mathf.SmoothDamp(
            velocity.x,
            targetVelocityX,
            ref VelocityXSmoothing,
            smoothTime
        );
    }

    private void Gravity()
    {
        if (!gravityEnabled)
            return;

        velocity.y += localGravity * Time.deltaTime;
    }

    private void ProcessThrow()
    {
        if (throwFired)
        {
            //skip frame incase entity is already grounded
            if (frameSkipper > 0)
            {
                frameSkipper--;
                return;
            }

            if (collisionInfo.below || collisionInfo.left || collisionInfo.right)
            {
                velocity.x = 0;
                throwFired = false;
            }
        }
    }

    private void ProcessJump()
    {
        if (jumpFired)
        {
            if (collisionInfo.below)
            {
                //soundComponent.PlaySound(jumpSoundData);
                // soundComponent.PlaySound(jumpVocalSoundData);
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
        if (!movementEnabled) return;

        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.velocityOld = velocity;

        // Only descend slope if ground is directly below center ray
        if (ShouldDescendSlope(velocity))
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);

        // Add a downward nudge to help stick to slopes when falling or going down slopes quickly
        if (gravityEnabled && !collisionInfo.below && velocity.y <= 0)
        {
            const float stickToGroundNudge = 3f; // You can tweak this value (e.g. 2f or 3f)
            velocity.y -= stickToGroundNudge * Time.deltaTime;
        }

        VerticalCollisions(ref velocity);

        transform.Translate(velocity);
        Physics2D.SyncTransforms(); //sync all child objects with parent object
    }

    private bool ShouldDescendSlope(Vector3 velocity)
    {
        var rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (GetBounds().size.x / 2f);
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            return slopeAngle > 0 && slopeAngle <= data.MaxDescendAngle;
        }

        return false;
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        if (velocity.x == 0)
            return;

        var directionX = Mathf.Sign(velocity.x);
        var rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            foreach (var hit in hits)
            {

                // Ignore colliders if they are the entity's own colliders
                if (collisionComponent.IsEntityCollider(hit.collider)) continue;

                //Ignore colliders if they are on the ignore list
                if (EntityCollisionService.IsIgnoredCollider(entity, hit.collider)) continue;

                if (hit)
                {
                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    // Handle slope climbing logic
                    if (i == 0 && slopeAngle <= data.MaxClimbAngle)
                    {
                        if (collisionInfo.descendingSlope)
                        {
                            collisionInfo.descendingSlope = false;
                            velocity = collisionInfo.velocityOld;
                        }

                        var distanceToSlopeStart = 0f;

                        // Smoother transition when slope angle changes
                        if (slopeAngle != collisionInfo.oldSlopeAngle)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            velocity.x -= distanceToSlopeStart * directionX;

                            // Gradual reset of slope data when reaching flat ground
                            if (slopeAngle == 0)
                            {
                                collisionInfo.climbingSlope = false;
                                collisionInfo.slopeAngle = 0;
                            }
                        }

                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    // Adjust for horizontal collisions when not climbing slopes
                    if (!collisionInfo.climbingSlope || slopeAngle > data.MaxClimbAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        // Smooth out vertical adjustment when transitioning to flat ground
                        if (collisionInfo.climbingSlope && slopeAngle == 0)
                        {
                            velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                            collisionInfo.climbingSlope = false;
                            collisionInfo.slopeAngle = 0;
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
        if (velocity.y == 0)
            return;

        var directionY = Mathf.Sign(velocity.y);
        var rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            var rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            foreach (var hit in hits)
            {

                //ignore colliders if they are entities own colliders
                if (collisionComponent.IsEntityCollider(hit.collider)) continue;

                //Ignore colliders if they are on the ignore list
                if (EntityCollisionService.IsIgnoredCollider(entity, hit.collider)) continue;

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
            var rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            foreach (var hit in hits)
            {

                //ignore colliders if they are entities own colliders
                if (collisionComponent.IsEntityCollider(hit.collider)) continue;

                //Ignore colliders if they are on the ignore list
                if (EntityCollisionService.IsIgnoredCollider(entity, hit.collider)) continue;

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
        if (velocity.y >= 0) return;

        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        // Cast diagonally down toward where the entity will go
        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            new Vector2(directionX, -1f).normalized,
            Mathf.Infinity,
            collisionMask
        );

        if (hit && !collisionComponent.IsEntityCollider(hit.collider) &&
            !EntityCollisionService.IsIgnoredCollider(entity, hit.collider))
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle <= data.MaxDescendAngle)
            {
                // Calculate distance the player will move horizontally
                float moveDistance = Mathf.Abs(velocity.x);
                float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * moveDistance)
                {
                    velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * directionX;
                    velocity.y -= descendVelocityY;

                    collisionInfo.slopeAngle = slopeAngle;
                    collisionInfo.descendingSlope = true;
                    collisionInfo.below = true;
                }
            }
        }
    }

    void CalculateRaySpacing()
    {
        var bounds = GetBounds();

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = (bounds.size.x - (verticalRayOffset.x * 2)) / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins()
    {
        var bounds = GetBounds();

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);


        //ray offsetting is very patchy, needs to be looked at again

        raycastOrigins.bottomLeft += new Vector2(verticalRayOffset.x, verticalRayOffset.y);
        raycastOrigins.bottomRight += new Vector2(-verticalRayOffset.x, verticalRayOffset.y);

        raycastOrigins.topLeft += new Vector2(verticalRayOffset.x, -verticalRayOffset.y);
        raycastOrigins.topRight += new Vector2(-verticalRayOffset.x, -verticalRayOffset.y);
    }

    private Bounds GetBounds()
    {
        var bounds = collisionComponent.GetEntityMainColliderBounds();
        bounds.Expand(skinWidth * -2);
        return bounds;
    }

}

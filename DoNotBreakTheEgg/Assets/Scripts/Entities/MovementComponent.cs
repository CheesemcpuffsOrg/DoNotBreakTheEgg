using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class MovementComponent : MonoBehaviour, IMovementComponent
{
    private struct CollisionInfoStruct
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = below = false;
            right = left = false;
        }
    }

    [SerializeField] LayerMask collisionMask;

    [SerializeField] float jumpHeight = 4;
    [SerializeField] float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;

    //[SerializeField] float gravityModifier = 1f;
    [SerializeField] float moveSpeed = 5f;
    
    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float VelocityXSmoothing;

    Vector2 input;
    bool jumpFired;

    float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    IEntity entity;

    CollisionInfoStruct collisionInfo;

    float skinWidth;

    ICollisionComponent collisionComponent;

    private void Start()
    {
        entity = GetComponent<IEntity>();

        collisionComponent = entity.GetEntityComponent<ICollisionComponent>();

        skinWidth = collisionComponent.SkinWidth;

        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        Debug.Log($"Gravity: " + gravity + "Jump Velocity: " +  jumpVelocity);
    }

    private void Update()
    {
        if (collisionInfo.above || collisionInfo.below)
        {
            velocity.y = 0;
        }

        ProcessJump();

        var targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX,  ref VelocityXSmoothing, (collisionInfo.below)?accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        Move(velocity * Time.deltaTime);
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
        collisionComponent.UpdateRaycastOrigins();
        collisionInfo.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }


        transform.Translate(velocity);
        Physics2D.SyncTransforms();
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        var directionX = Mathf.Sign(velocity.x);
        var rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < collisionComponent.HorizontalRayCount; i++)
        {
            var rayOrigin = (directionX == -1) ? collisionComponent.RaycastOrigins.bottomLeft : collisionComponent.RaycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (collisionComponent.HorizontalRaySpacing * i);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);


            foreach(var hit in hits)
            {
                if (collisionComponent.AllPlayerColliders.Contains(hit.collider))
                {
                    continue;
                }

                // Check if the raycast hit something
                if (hit)
                {
                    // Process the valid hit (e.g., other players or ground)
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;


                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }
            }   
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        var directionY = Mathf.Sign(velocity.y);
        var rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < collisionComponent.VerticalRayCount; i++)
        {
            var rayOrigin = (directionY == -1) ? collisionComponent.RaycastOrigins.bottomLeft : collisionComponent.RaycastOrigins.topleft;
            rayOrigin += Vector2.right * (collisionComponent.VerticalRaySpacing * i + velocity.x);
            var hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            //Debug.DrawRay(collisionComponent.RaycastOrigins.bottomLeft + Vector2.right * collisionComponent.VerticalRaySpacing * i, Vector2.up * -2, Color.red);

            foreach (var hit in hits)
            {
                if (collisionComponent.AllPlayerColliders.Contains(hit.collider))
                {
                    continue;
                }
                
                // Process the valid hit
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;

                break;
            }
        }
    }

    public void Jump()
    {
        jumpFired = true;
        jumpBufferCounter = jumpBufferTime;
    }

    public void SetTarget(Vector2 target)
    {
         input = target;
    }

    public void StopMovement()
    {
        input = Vector2.zero;
    }


    /*[Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Wall Collisions")]
    [SerializeField] private LayerMask collisionLayer; // Layers to check for collisions
    [SerializeField] private Vector2 colliderSize = new Vector2(1f, 1f); // Size of your character's collider
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] float skinWidth = 0.01f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -20f; // Simulated gravity
    [SerializeField] LayerMask canJump;
    [SerializeField] Transform groundCheck; // Ground check position
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.5f); // Ground check radius
    [SerializeField] float jumpBufferTime = 0.2f; // Time to buffer jump input

    [Header("Tags")]
    [SerializeField] TagScriptableObject isGroundedTag;
    [SerializeField] TagFilter moveFilter;
    [SerializeField] TagFilter jumpFilter;

    private float localDirection;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isJumping;
    private float jumpBufferCounter; // Tracks remaining buffer time

    IEntity entity;

    private void Start()
    {
        entity = GetComponent<IEntity>();
    }

    private void Update()
    {
        CheckGrounded();

        bool canMove = moveFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>().GetTransform());
        Debug.Log($"Move filter result: {canMove} for entity {entity}");

        if (moveFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>().GetTransform()))
        {
            Movement();
        }
            
        ProcessJumpBuffer();
    }

    public void SetTarget(Vector2 position)
    {
        localDirection = position.x; // Capture horizontal movement input
    }

    public void StopMovement()
    {
        localDirection = 0f; // Reset input when movement is cancelled
    }

    public void Jump()
    {
        if (isGrounded && jumpFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>().GetTransform()))
        {
            // Calculate jump velocity and reset vertical velocity
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight); // Calculate initial jump velocity
            isJumping = true; // Set jumping state
            jumpBufferCounter = 0; // Clear the jump buffer after jump
        }
        else
        {
            // Buffer jump if not grounded
            jumpBufferCounter = jumpBufferTime;
        }
    }

    private void ProcessJumpBuffer()
    {
        // Decrease buffer time, and trigger jump when grounded
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (jumpBufferCounter <= 0 && isGrounded && jumpFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>().GetTransform()))
            {
                Jump(); // Execute buffered jump when grounded
            }
        }
    }

    private void Movement()
    {
        // Horizontal movement
        Vector3 horizontalMovement = new Vector2(localDirection * moveSpeed * Time.deltaTime, 0f);
        Vector2 newPosition = transform.position + horizontalMovement;

        // Vertical movement (gravity is applied only if airborne)
        if (isJumping || !isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity if airborne
            entity.GetEntityComponent<ITagComponent>().RemoveTag(isGroundedTag);
        }
        else
        {
            verticalVelocity = 0f; // Stop vertical movement when grounded

            if (ThrowEntityManager.Instance.IsEntityBeingThrown(entity))
            {
                ThrowEntityManager.Instance.RemoveThrownEntity(entity);
            }

            entity.GetEntityComponent<ITagComponent>().AddTag(isGroundedTag);
            
        }

        Vector3 verticalMovement = new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

        Vector3 totalMovement = CollisionHandling(horizontalMovement, verticalMovement, newPosition);

        // Apply the resolved movement
        transform.Translate(totalMovement, Space.World);
        //transform.Translate(horizontalMovement + verticalMovement);
    }

    private Vector3 CollisionHandling(Vector3 horizontalMovement, Vector3 verticalMovement, Vector2 newPosition)
    {
        

        // Adjust collider size for skin width
        Vector2 adjustedColliderSize = colliderSize - new Vector2(skinWidth * 2, skinWidth * 2);

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(collisionLayer);
        filter.useTriggers = false; // Ignore triggers, if needed

        Collider2D[] results = new Collider2D[10];
        int count = Physics2D.OverlapBox(newPosition, adjustedColliderSize, 0f, filter, results);

        // Combine horizontal and vertical movement
        Vector3 totalMovement = horizontalMovement + verticalMovement;

        // Resolve collisions
        for (int i = 0; i < count; i++)
        {
            Collider2D hitCollider = results[i];

            if (HoldEntityManager.Instance.IsEntityHeld(entity) &&
           HoldEntityManager.Instance.GetHeldEntity(entity).GetEntityComponent<IGameObjectComponent>().GetTransform().GetComponent<Collider2D>() == hitCollider)
            {
                continue; // Skip collision resolution for held entity
            }

            if (!colliders.Any(c => c == hitCollider))
            {
                // Calculate collision normal
                Vector2 collisionNormal = (Vector2)transform.position - hitCollider.ClosestPoint(transform.position);

                // Project total movement onto the plane perpendicular to the collision normal
                totalMovement -= Vector3.Project(totalMovement, collisionNormal);
            }  
        }

        return totalMovement;
    }

    private void CheckGrounded()
    {

        // Check if the player is grounded
        bool wasGrounded = isGrounded;

        // OverlapBox to detect colliders in the ground layer
        Collider2D[] groundColliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f, canJump);

        // Ensure the ground is below the player
        isGrounded = false;
        foreach (var collider in groundColliders)
        {
            if (collider != null)
            {
                // Get the bottom-most point of the player and compare it to the collider's position
                float playerBottomY = groundCheck.position.y;
                if (collider.bounds.max.y <= playerBottomY) // Collider is below the player
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (isGrounded && !wasGrounded)
        {
            // Player just landed
            isJumping = false; // Reset jumping state on landing
            verticalVelocity = 0f; // Stop vertical velocity to avoid bobbing
            if (jumpBufferCounter > 0)
            {
                Jump(); // Trigger buffered jump when landing
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        Gizmos.color = Color.red; // Set the color for the box

        // Calculate the box's position based on the player's position
        Vector3 boxPosition = transform.position;

        // Draw the wireframe box in the Scene view
        Gizmos.DrawWireCube(boxPosition, colliderSize);
    }

#endif*/

}

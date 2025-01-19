using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementComponent : MonoBehaviour, IMovementComponent
{

    [SerializeField] PlayerStateController playerStateController;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -20f; // Simulated gravity
    [SerializeField] LayerMask canJump;
    [SerializeField] Transform groundCheck; // Ground check position
    [SerializeField] Vector2 groundCheckRadius = new Vector2(0.5f, 0.5f); // Ground check radius
    [SerializeField] float jumpBufferTime = 0.2f; // Time to buffer jump input

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
        Movement();
        ProcessJumpBuffer();
    }

    public void MoveAlongXAxis(float direction)
    {
        localDirection = direction; // Capture horizontal movement input
    }

    public void StopXAxisMovement()
    {
        localDirection = 0f; // Reset input when movement is cancelled
    }

    public void Jump()
    {
        if (isGrounded)
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

    public void StopAllMovement()
    {
        StopXAxisMovement();
        playerStateController.IsThrown(false);
        verticalVelocity = 0f;
    }

    private void ProcessJumpBuffer()
    {
        // Decrease buffer time, and trigger jump when grounded
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (jumpBufferCounter <= 0 && isGrounded)
            {
                Jump(); // Execute buffered jump when grounded
            }
        }
    }

    private void Movement()
    {
        // Horizontal movement
        Vector3 horizontalMovement = new Vector2(localDirection * moveSpeed * Time.deltaTime, 0f);

        bool test = false;

        // Vertical movement (gravity is applied only if airborne)
        if (isJumping || !isGrounded)
        {
            if (!HoldEntityManager.Instance.IsEntityHeld(entity))
            {
                verticalVelocity += gravity * Time.deltaTime; // Apply gravity if airborne
            } 
        }
        else
        {
            verticalVelocity = 0f; // Stop vertical movement when grounded

            if (ThrowEntityManager.Instance.IsEntityBeingThrown(entity))
            {
                ThrowEntityManager.Instance.RemoveThrownEntity(entity);
            }    
        }

        Vector3 verticalMovement = new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

        // Apply combined movement (horizontal + vertical)
        transform.Translate(horizontalMovement + verticalMovement);
    }

    private void CheckGrounded()
    {
        // Check if the player is grounded
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckRadius, 0f, canJump);

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
            Gizmos.DrawWireCube(groundCheck.position, groundCheckRadius);
        }
    }
#endif
}

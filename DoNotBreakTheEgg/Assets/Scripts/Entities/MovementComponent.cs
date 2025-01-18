using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour, IEntityComponent
{
    [SerializeField] PlayerInputController controller;
    [SerializeField] PlayerStateController stateController;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -20f; // Simulated gravity
    [SerializeField] LayerMask canJump;
    [SerializeField] Transform groundCheck; // Ground check position
    [SerializeField] Vector2 groundCheckRadius = new Vector2(0.5f, 0.5f); // Ground check radius
    [SerializeField] float jumpBufferTime = 0.2f; // Time to buffer jump input

    private float input;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isJumping;
    private float jumpBufferCounter; // Tracks remaining buffer time

    private void Update()
    {
        CheckGrounded();
        Movement();
        ProcessJumpBuffer();
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
        Vector3 horizontalMovement = new Vector2(input * moveSpeed * Time.deltaTime, 0f);

        // Vertical movement (gravity is applied only if airborne)
        if (isJumping || !isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity if airborne
        }
        else
        {
            verticalVelocity = 0f; // Stop vertical movement when grounded
        }

        Vector3 verticalMovement = new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

        // Apply combined movement (horizontal + vertical)
        transform.Translate(horizontalMovement + verticalMovement);
    }

    private void Jump()
    {

        if (!stateController.CanJump())
            return;

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

    private void MovePerformed(Vector2 context)
    {
        input = context.x; // Capture horizontal movement input
    }

    private void MoveCancelled()
    {
        input = 0f; // Reset input when movement is cancelled
    }

    private void OnEnable()
    {
        controller.MoveEventPerfomed += MovePerformed;
        controller.MoveEventCancelled += MoveCancelled;
        controller.JumpEventPerformed += Jump;
    }

    private void OnDisable()
    {
        controller.MoveEventPerfomed -= MovePerformed;
        controller.MoveEventCancelled -= MoveCancelled;
        controller.JumpEventPerformed -= Jump;
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

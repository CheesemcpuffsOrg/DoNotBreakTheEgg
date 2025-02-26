using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    IEntity entity;
    IMovementComponent movementComponent;

    [SerializeField] AnimationController2D controller;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Animations")]
    [SerializeField] string idleAnimation;
    [SerializeField] string movementAnimation;
    [SerializeField] string jumpAnimation;
    [SerializeField] string fallAnimation;

    [Header("Tags")]
    [SerializeField] TagFilter groundedFilter;

    bool isMoving;

    private void Start()
    {
        entity = GetComponent<IEntity>();
        movementComponent = entity.GetEntityComponent<IMovementComponent>();
    }


    private void Update()
    {
        isMoving = movementComponent.Velocity.magnitude > 2f;

        FlipSpriteDirection();

        if (entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(groundedFilter))
        {
            if (!isMoving)
            {
                if (!controller.IsAnimationPlaying(idleAnimation))
                {
                    controller.PlayAnimation(idleAnimation);
                }
            }
            else
            {
                if (!controller.IsAnimationPlaying(movementAnimation))
                {
                    controller.PlayAnimation(movementAnimation);
                }
            }
        }
        else
        {
            if (movementComponent.Velocity.normalized.y > 0)
            {
                controller.PlayAnimation(jumpAnimation);
            }
            else if (movementComponent.Velocity.normalized.y < 0)
            {
                controller.PlayAnimation(fallAnimation);
            }
        }

    }

    private void FlipSpriteDirection()
    {
        if (movementComponent.Velocity.normalized.x > 0)
        {
            spriteRenderer.flipX = false;

        }
        else if (movementComponent.Velocity.normalized.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}

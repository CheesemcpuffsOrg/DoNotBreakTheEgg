using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour, IEntityComponent
{
    [SerializeField] PlayerInputController controller;

    [SerializeField] float moveSpeed = 5f;

    private float input;

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = new Vector2(input * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(movement);
    }

    private void MovePerformed(Vector2 context)
    {
        input = context.x;
    }

    private void MoveCancelled()
    {
        input = 0f;
    }

    private void OnEnable()
    {
        controller.MoveEventPerfomed += MovePerformed;
        controller.MoveEventCancelled += MoveCancelled;
    }

    private void OnDisable()
    {
        controller.MoveEventPerfomed -= MovePerformed;
        controller.MoveEventCancelled -= MoveCancelled;
    }
}

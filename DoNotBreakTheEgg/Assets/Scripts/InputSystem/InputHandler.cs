using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] InputController controller;
    [SerializeField] MonoEntity playerEntity;

    IEntity entity => playerEntity.GetComponent<IEntity>();

    private void ThrowEventStarted()
    {
        entity.GetEntityComponent<IThrowAndCatchComponent>().ChargeThrow();
    }

    private void ThrowEventPerformed()
    {
        entity.GetEntityComponent<IThrowAndCatchComponent>().Throw();
    }

    private void AimEventPerformed(Vector2 position)
    {
        entity.GetEntityComponent<IAimComponent>().MoveAim(position);
    }

    private void AimEventCancelled()
    {
        entity.GetEntityComponent<IAimComponent>().StopAim();
    }

    private void MoveEventPerformed(Vector2 position)
    {
        entity.GetEntityComponent<IMovementComponent>().SetTarget(position);
    }

    private void MoveEventCancelled()
    {
        entity.GetEntityComponent<IMovementComponent>().StopMovement();
    }

    private void JumpEventPerformed()
    {
        entity.GetEntityComponent<IMovementComponent>().Jump();
    }


    private void OnEnable()
    {
        controller.ThrowEventStarted += ThrowEventStarted;
        controller.ThrowEventPerformed += ThrowEventPerformed;
        controller.AimEventPerformed += AimEventPerformed;
        controller.AimEventCancelled += AimEventCancelled;
        controller.MoveEventPerfomed += MoveEventPerformed;
        controller.MoveEventCancelled += MoveEventCancelled;
        controller.JumpEventPerformed += JumpEventPerformed;
    }

    private void OnDisable()
    {
        controller.ThrowEventStarted -= ThrowEventStarted;
        controller.ThrowEventPerformed -= ThrowEventPerformed;
        controller.AimEventPerformed -= AimEventPerformed;
        controller.AimEventCancelled -= AimEventCancelled;
        controller.MoveEventPerfomed -= MoveEventPerformed;
        controller.MoveEventCancelled -= MoveEventCancelled;
        controller.JumpEventPerformed -= JumpEventPerformed;
    }
}

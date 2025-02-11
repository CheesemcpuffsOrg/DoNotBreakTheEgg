using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementComponent : IEntityComponent
{
    Vector3 Velocity { get; }

    public void Move(Vector2 position);

    public void StopMovement();

    public void Jump();

    public void Throw(float power, Vector2 direction);

    public void EnableGravity();
    public void DisableGravity();
}

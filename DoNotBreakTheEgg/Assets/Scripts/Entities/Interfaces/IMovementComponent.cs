using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementComponent : IEntityComponent
{
    public void SetTarget(Vector2 position);

    public void StopMovement();

    public void Jump();
}

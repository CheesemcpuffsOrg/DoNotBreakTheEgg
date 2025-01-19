using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementComponent : IEntityComponent
{
    public void MoveAlongXAxis(float direction);

    public void StopXAxisMovement();

    public void Jump();
}

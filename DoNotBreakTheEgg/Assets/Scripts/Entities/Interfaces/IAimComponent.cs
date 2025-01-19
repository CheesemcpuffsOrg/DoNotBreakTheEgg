using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAimComponent : IEntityComponent
{
    public void MoveAim(Vector2 direction);

    public void StopAim();
}

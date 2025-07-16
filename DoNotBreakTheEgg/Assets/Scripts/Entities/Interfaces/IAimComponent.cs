using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IAimComponent : IEntityComponent
{
    public void MoveAim(Vector2 direction);

    public void StopAim();

    public void SetInputDevice(InputDevice inputDevice);
}

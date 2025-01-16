using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IThrowableComponent : IEntityComponent
{
    void Throw(float power, Transform startPoint);
}

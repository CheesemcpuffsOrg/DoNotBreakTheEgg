using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldableComponent : IEntityComponent
{
    public void Hold(Transform anchor);

    public void Release();
}

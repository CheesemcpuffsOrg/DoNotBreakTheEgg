using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowComponent : IEntityComponent
{
    public void ChargeThrow();

    public void Throw();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowAndCatchComponent : IEntityComponent
{
    public void ChargeThrow();

    public void Throw();
}

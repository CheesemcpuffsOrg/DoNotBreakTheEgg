using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityEmissionStrategy
{
    public void Emit(IEntity entity);
}

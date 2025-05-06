using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEmissionStrategy
{

    public void Emit();

    public void Emit(IEntity entity);

    
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AndEmissionDecorator : EmissionBase<object>
{

    [SerializeField] GameObject[] emissionStrategiesObjects;

    private IEmission[] emissionStrategies => emissionStrategiesObjects.Select(obj => obj.GetComponent<IEmission>()).ToArray();

    public override void Emit(object obj)
    {      
        foreach (var emissionStrategy in emissionStrategies)
        {
            emissionStrategy.Emit(obj);
        }
    }
}

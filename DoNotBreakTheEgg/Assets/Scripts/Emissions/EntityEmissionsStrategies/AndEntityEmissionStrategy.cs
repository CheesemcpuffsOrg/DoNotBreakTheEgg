using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AndEntityEmissionStrategy : MonoBehaviour, IEntityEmissionStrategy
{


    [SerializeField] GameObject[] emissionStrategiesObjects;

    private IEntityEmissionStrategy[] emissionStrategies => emissionStrategiesObjects.Select(obj => obj.GetComponent<IEntityEmissionStrategy>()).ToArray();

    public void Emit(IEntity entity)
    {
        foreach (var emissionStrategy in emissionStrategies)
        {
            Debug.Log(entity);
            Debug.Log(emissionStrategy);
            emissionStrategy.Emit(entity);
        }
    }
}

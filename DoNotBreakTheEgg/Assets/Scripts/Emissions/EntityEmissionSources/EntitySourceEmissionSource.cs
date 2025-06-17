using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySourceEmissionSource : MonoBehaviour  
{
    

    [SerializeField] GameObject entitySourceObj;
    IEntitySource entitySource => entitySourceObj.GetComponent<IEntitySource>();


    [SerializeField] GameObject entityEmissionStrategyObj;

    IEntityEmissionStrategy entityEmissionStrategy => entityEmissionStrategyObj.GetComponent<IEntityEmissionStrategy>();

    // Start is called before the first frame update
    void Start()
    {
        
        var subscriptionBag = Disposable.CreateBuilder();

      
        entitySource
            .Entities
            .Subscribe(entity =>
            {
                entityEmissionStrategy.Emit(entity);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

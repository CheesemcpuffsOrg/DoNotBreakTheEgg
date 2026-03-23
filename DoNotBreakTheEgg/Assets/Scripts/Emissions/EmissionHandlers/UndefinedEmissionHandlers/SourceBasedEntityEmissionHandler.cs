using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SourceBasedEntityEmissionHandler : MonoBehaviour  
{
    
    [SerializeField] GameObject entitySourceObj;
    ISource entitySource => entitySourceObj.GetComponent<ISource>();


    [SerializeField] GameObject entityEmissionStrategyObj;

    IEmission entityEmissionStrategy => entityEmissionStrategyObj.GetComponent<IEmission>();

    // Start is called before the first frame update
    void Start()
    {
        
        var subscriptionBag = Disposable.CreateBuilder();

      
        entitySource
            .Gained
            .Subscribe(entity =>
            {
                entityEmissionStrategy.Emit(entity);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

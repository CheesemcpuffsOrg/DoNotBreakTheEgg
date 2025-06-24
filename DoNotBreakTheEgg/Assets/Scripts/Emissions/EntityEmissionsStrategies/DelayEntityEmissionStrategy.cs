using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEntityEmissionStrategy : MonoBehaviour, IEntityEmissionStrategy
{

    [SerializeField] float delay;

    [SerializeField] GameObject emissionStrategyObj;

    private IEntityEmissionStrategy emissionStrategy => emissionStrategyObj.GetComponent<IEntityEmissionStrategy>();

    Subject<IEntity> emissionDelay = new Subject<IEntity> ();

    private void Awake()
    {
        var subscriptionBag = Disposable.CreateBuilder();

        emissionDelay
            .Select(entity =>
            {
                return Observable
                    .Timer(TimeSpan.FromSeconds(delay))
                    .Select(_ => entity);
            })
            .Switch()
            .Subscribe(entity =>
            {
                emissionStrategy.Emit(entity);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

    public void Emit(IEntity entity)
    {
        emissionDelay.OnNext(entity);
    }

    
}

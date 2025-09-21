using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEmissionDecorator : EmissionBase<object>
{

    [SerializeField] float delay;

    [SerializeField] GameObject emissionStrategyObj;

    private IEmission emissionStrategy => emissionStrategyObj.GetComponent<IEmission>();

    Subject<object> emissionDelay = new Subject<object> ();

    private void Awake()
    {
        var subscriptionBag = Disposable.CreateBuilder();

        emissionDelay
            .Select(obj =>
            {
                return Observable
                    .Timer(TimeSpan.FromSeconds(delay))
                    .Select(_ => obj);
            })
            .Switch()
            .Subscribe(obj =>
            {
                emissionStrategy.Emit(obj);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

    public override void Emit(object obj)
    {
        emissionDelay.OnNext(obj);
    }

    
}

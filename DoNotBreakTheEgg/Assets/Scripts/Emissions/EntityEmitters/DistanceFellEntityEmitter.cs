using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFellEntityEmitter : MonoBehaviour
{

    [SerializeField] float distanceToTravel;


    [Header("Entity Sources")]
    [SerializeField] GameObject airbornEntitySourceObj;
    [SerializeField] GameObject groundedEntitySourceObj;

    IEntitySource airbornEntitySource => airbornEntitySourceObj.GetComponent<IEntitySource>();
    IEntitySource groundedEntitySource => groundedEntitySourceObj.GetComponent<IEntitySource>();


    [Header("Emission Strategy")]
    [SerializeField] GameObject entityEmissionStrategyObj;

    IEntityEmissionStrategy entityEmissionStrategy => entityEmissionStrategyObj.GetComponent<IEntityEmissionStrategy>();

    // Start is called before the first frame update
    void Start()
    {
        var subscriptionBag = Disposable.CreateBuilder();
        
        var sourceTwo = new Subject<IEntity>();
        var sourceOne = new Subject<IEntity>();
        var thresholdReached = new Subject<IEntity>();

        groundedEntitySource
            .Entities
            .Subscribe(entity =>
            {
                sourceTwo.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);

        airbornEntitySource
            .Entities
            .Subscribe(entity =>
            {
               sourceOne.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);


        sourceOne
            .SelectMany(entity =>
            {
                Vector3? startPosition = null;
                bool startedDescending = false;

                return Observable
                    .EveryUpdate(UnityFrameProvider.PostLateUpdate)
                    .Where(_ =>
                    {
                        Vector3 currentPos = entity.GetEntityComponent<IAnchoringComponent>().GetPosition();

                        if (startPosition == null)
                            startPosition = currentPos;

                        if (!startedDescending && currentPos.y < startPosition.Value.y)
                            startedDescending = true;

                        if (startedDescending)
                        {
                            float distanceDescended = startPosition.Value.y - currentPos.y;
                            return distanceDescended >= 1;
                        }

                        return false;
                    })
                    .Take(1) // emit only once, then complete)
                    .Select(_ => entity)
                    .TakeUntil(sourceTwo.Where(e => e == entity)); // you can select what you want to emit during tracking
            })
            .Subscribe(entity =>
            {
                thresholdReached.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);

        thresholdReached
            .SelectMany(entity =>
            {
                return sourceTwo
                    .Where(e => e == entity)
                    .Take(1);
            })
            .Subscribe(entity =>
            {
                entityEmissionStrategy.Emit(entity);
            })
            .AddTo(ref subscriptionBag);
        

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

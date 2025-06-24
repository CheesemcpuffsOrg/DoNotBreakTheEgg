using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFellEntityEmissionSource : MonoBehaviour
{

    [SerializeField] float distanceToTravel;


    [Header("Entity Sources")]
    [SerializeField] GameObject airbornEntitySourceObj;

    IEntitySource airbornEntitySource => airbornEntitySourceObj.GetComponent<IEntitySource>();


    [Header("Emission Strategy")]
    [SerializeField] GameObject entityEmissionStrategyObj;

    IEntityEmissionStrategy entityEmissionStrategy => entityEmissionStrategyObj.GetComponent<IEntityEmissionStrategy>();

    // Start is called before the first frame update
    void Start()
    {
        var subscriptionBag = Disposable.CreateBuilder();
        
        var lostSource = new Subject<IEntity>();
        var gainedSource = new Subject<IEntity>();
        var thresholdReached = new Subject<IEntity>();

        airbornEntitySource
            .LostEntities
            .Subscribe(entity =>
            {
                lostSource.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);


        airbornEntitySource
            .Entities
            .Subscribe(entity =>
            {
               gainedSource.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);


        gainedSource
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
                    .TakeUntil(lostSource.Where(e => e == entity)); // you can select what you want to emit during tracking
            })
            .Subscribe(entity =>
            {
                thresholdReached.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);

        thresholdReached
            .SelectMany(entity =>
            {
                return lostSource
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

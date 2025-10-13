using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceFellEntityEmissionHandler : MonoBehaviour
{

    [SerializeField] float distanceToTravel;


    [Header("Entity Sources")]
    [SerializeField] EntitySource airbornEntitySource;

    [Header("Emission Strategy")]
    [SerializeField] GameObject entityEmissionStrategyObj;

    IEmission _entityEmission;

    IEmission entityEmissionStrategy => _entityEmission ??= entityEmissionStrategyObj.GetComponent<IEmission>();

    private CompositeDisposable subscriptionBag;

    // Start is called before the first frame update
    void Start()
    {

        subscriptionBag = new CompositeDisposable();


        var apexReached = new Subject<IEntity>();

        airbornEntitySource
            .GainedEntities
            .SelectMany(entity =>
            {
              
                var anchoring = entity.GetEntityComponent<IAnchoringComponent>();

                var startPosition = anchoring.GetPosition();

                return Observable
                    .EveryUpdate(UnityFrameProvider.PostLateUpdate)
                    .Select(_ => startPosition.y - anchoring.GetPosition().y >= distanceToTravel)
                    .Where(_ => _)
                    .Take(1)
                    .Select(_ => entity)
                    .TakeUntil(airbornEntitySource.LostEntities.Merge(entity.Destroyed.Where(e => e == entity)));
            })
            .Subscribe(entity =>
            {
                apexReached.OnNext(entity);
            });

        apexReached
            .SelectMany(entity =>
            {
                return airbornEntitySource
                    .LostEntities
                    .Where(e => e == entity)
                    .Take(1);
            })
            .Subscribe(entity =>
            {
                entityEmissionStrategy.Emit(entity);
            });


        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

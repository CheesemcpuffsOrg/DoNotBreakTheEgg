using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;
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

    private CompositeDisposable subscriptionBag;

    // Start is called before the first frame update
    void Start()
    {

        subscriptionBag = new CompositeDisposable();


        var apexReached = new Subject<IEntity>();

        airbornEntitySource
            .Entities
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

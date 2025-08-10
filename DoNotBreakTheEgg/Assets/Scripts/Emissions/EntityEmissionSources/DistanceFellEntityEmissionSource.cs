using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

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

        airbornEntitySource
            .Entities
            .SelectMany(entity =>
            {
                Debug.Log("It has fired");

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
                Debug.Log("What the fuck");
                // entityEmissionStrategy.Emit(entity);
            });

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

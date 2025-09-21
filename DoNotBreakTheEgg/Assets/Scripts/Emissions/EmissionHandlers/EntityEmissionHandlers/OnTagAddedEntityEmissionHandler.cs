using ObservableCollections;
using R3;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class OnTagAddedEntityEmissionHandler : MonoBehaviour
{
    [SerializeField] GameObject emissionStrategyObj;

    [SerializeField] TagScriptableObject tagSO;

    IEmission emissionStrategy;
    IEmission EmissionStrategy => emissionStrategy ??= emissionStrategyObj.GetComponent<IEmission>();

    // Start is called before the first frame update
    void Start()
    {
        var subscriptionBag = Disposable.CreateBuilder();

        EntityRegistry
            .RegisteredEntities
            .ObserveAdd()
            //.Where(addEvent => mappings.Any(mapping => addEvent.Value.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject)))
            .SelectMany(addEvent =>
            {
                var entity = addEvent.Value;
                var tagComponent = entity.GetEntityComponent<ITagComponent>();

                return Observable
                    .EveryUpdate()
                    .Select(_ => tagComponent.HasTag(tagSO))
                    .DistinctUntilChanged() // Only emit when value changes
                    .Pairwise() // Get the previous and current value as a tuple
                    .Where(pair => !pair.Previous && pair.Current) 
                    .Select(_ => entity)
                    .TakeUntil(entity.Destroyed.Where(destroyedEntity => destroyedEntity == entity));
            })
            .Subscribe(entity =>
            {
                EmissionStrategy.Emit(entity);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);

    }

}

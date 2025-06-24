using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagBasedEntitySource : MonoBehaviour, IEntitySource
{

    [SerializeField] TagFilter filter;

    private Dictionary<IEntity, bool> _entityPassStates = new();

    public Observable<IEntity> Entities =>
        Observable.Defer(() =>
        {// 1. Fresh snapshot of currently passing entities at subscription time
            var currentPassingEntities = EntityRegistry.GetRegisteredEntities()
                .Where(e => e.GetEntityComponent<ITagComponent>().PassTagFilterCheck(filter))
                .ToObservable();

            // 2. Future entities added (all entities)
            var futureEntities = EntityRegistry.RegisteredEntities.ObserveAdd()
                .Select(evt => evt.Value);

            // 3. All entities to listen to (current + future)
            var allEntities = EntityRegistry.GetRegisteredEntities()
                .ToObservable()
                .Concat(futureEntities);

            // 4. Observable emitting when entities newly pass the filter
            var tagUpdates = allEntities
                .SelectMany(entity =>
                {
                    var tagComponent = entity.GetEntityComponent<ITagComponent>();

                    // Initialize lastPassed only once per entity globally
                    if (!_entityPassStates.ContainsKey(entity))
                    {
                        _entityPassStates[entity] = tagComponent.PassTagFilterCheck(filter);
                    }

                    return tagComponent.TagAddedStream
                        .Merge(tagComponent.TagRemovedStream)
                        .Select(_ =>
                        {
                            bool nowPasses = tagComponent.PassTagFilterCheck(filter);
                            bool lastPassed = _entityPassStates[entity];
                            bool shouldEmit = !lastPassed && nowPasses;

                            _entityPassStates[entity] = nowPasses;
                            return shouldEmit ? entity : null;
                        })
                        .Where(e => e != null);
                });

            // 5. Future entities that already pass filter on add
            var futurePassingEntities = futureEntities
                .Where(e => e.GetEntityComponent<ITagComponent>().PassTagFilterCheck(filter));

            // 6. Combine:
            // - fresh current snapshot at subscription
            // - future entities that already pass
            // - entities passing filter due to tag changes
            return currentPassingEntities
                .Concat(futurePassingEntities)
                .Merge(tagUpdates); // avoid duplicates
        });
}

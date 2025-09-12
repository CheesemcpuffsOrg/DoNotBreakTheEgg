using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagBasedEntitySource : MonoBehaviour, IEntitySource
{

    [SerializeField] TagFilter filter;

    //With how these streams are handled, it is a good idea to subscribe to them instantly and fire off the emissions you obtain with a subject.  

    public Observable<IEntity> Entities =>
        Observable.Defer(() =>
        {
            var currentPassingEntities = EntityRegistry.GetRegisteredEntities()
                .Where(e => e.GetEntityComponent<ITagComponent>().PassTagFilterCheck(filter))
                .ToObservable();

            var futureEntities = EntityRegistry.RegisteredEntities.ObserveAdd()
                .Select(evt => evt.Value)
                .Where(e => e.GetEntityComponent<ITagComponent>().PassTagFilterCheck(filter));

            var tagUpdates = EntityRegistry.GetRegisteredEntities()
                .ToObservable()
                .Concat(EntityRegistry.RegisteredEntities.ObserveAdd().Select(evt => evt.Value))
                .SelectMany(entity =>
                {
                    var tagComponent = entity.GetEntityComponent<ITagComponent>();

                    return Observable.Return(tagComponent.PassTagFilterCheck(filter)) // initial state emitted first
                        .Concat(
                            tagComponent.TagAddedStream
                                .Merge(tagComponent.TagRemovedStream)
                                .Select(_ => tagComponent.PassTagFilterCheck(filter))
                        )
                        .Scan((prev: false, curr: false), (acc, now) => (acc.curr, now))
                        .Where(p => !p.prev && p.curr) // transition from not passing to passing
                        .Select(_ => entity);
                });

            return currentPassingEntities
                .Concat(futureEntities)
                .Merge(tagUpdates);
        })
        .Share();

    public Observable<IEntity> LostEntities =>
        Entities
            .SelectMany(entity =>
            {
                var tagComponent = entity.GetEntityComponent<ITagComponent>();

                return Observable.Return(tagComponent.PassTagFilterCheck(filter)) // initial state emitted first
                    .Concat(
                        tagComponent.TagAddedStream
                            .Merge(tagComponent.TagRemovedStream)
                            .Select(_ => tagComponent.PassTagFilterCheck(filter))
                    )
                    .Scan((prev: true, curr: true), (acc, now) => (acc.curr, now))
                    .Where(p => p.prev && !p.curr) // transition from passing to not passing
                    .Take(1) // emit only once per pass/loss cycle
                    .Select(_ => entity);
            })
            .Share();

    public List<IEntity> PassingEntitySet => throw new System.NotImplementedException();
}

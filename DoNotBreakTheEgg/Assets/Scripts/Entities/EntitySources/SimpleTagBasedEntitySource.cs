using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class SimpleTagBasedEntitySource : MonoBehaviour, IEntitySource
{
    [SerializeField] private TagFilter filter;

    public List<IEntity> PassingEntitySet { get; private set; }

    Subject<IEntity> entityGained = new Subject<IEntity>();
    Subject<IEntity> entityLost = new Subject<IEntity>();

    private IDisposable subscriptionBag; // use this if you have less than 8 disposables
    public Observable<IEntity> Entities => entityGained;

    public Observable<IEntity> LostEntities => entityLost;

    private readonly HashSet<IEntity> currentlyGainedEntities = new();

    private void Start()
    {

        PassingEntitySet = new();

        var unregisteredEntities = EntityRegistry
            .RegisteredEntities
            .ObserveRemove()
            .Share();

        var disposable1 = EntityRegistry
            .RegisteredEntities
            .ToObservable()
            .Merge(EntityRegistry.RegisteredEntities.ObserveAdd().Select(addEvent => addEvent.Value))
            .SelectMany(entity =>
            {
                var tagComponent = entity.GetEntityComponent<ITagComponent>();

                var passedStream = tagComponent.TagAddedStream
                    .Merge(tagComponent.TagRemovedStream)
                    .Select(_ => tagComponent.PassTagFilterCheck(filter))
                    .DistinctUntilChanged()
                    .Select(passed => (entity, passed))
                    .TakeUntil(unregisteredEntities.Where(e => e.Value == entity));

                return passedStream;
            })
            .Subscribe(tuple =>
            {
                var (entity, passed) = tuple;

                if (passed)
                {
                    if (currentlyGainedEntities.Add(entity))
                    {
                        entityGained.OnNext(entity);
                        PassingEntitySet.Add(entity);
                    }

                }
                else
                {
                    if (currentlyGainedEntities.Remove(entity))
                    {
                        entityLost.OnNext(entity);
                        PassingEntitySet.Remove(entity);
                    }

                }
            });

        // Also handle entities removed from the list (unregistered)
        var disposable2 = unregisteredEntities
            .Subscribe(removeEvent =>
            {
                var entity = removeEvent.Value;

                entityLost.OnNext(entity);

                PassingEntitySet.Remove(entity);
            });

        subscriptionBag = Disposable.Combine(disposable1, disposable2);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

}

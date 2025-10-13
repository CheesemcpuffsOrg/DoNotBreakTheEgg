using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TagBasedEntitySource : EntitySource
{
    [SerializeField] private TagFilter filter;

   /* // Strongly-typed observable for subscribers who know it's IEntity
    public Observable<IEntity> GainedEntities => gainedSubject;
    public Observable<IEntity> LostEntities => lostSubject;

    // Optional: override base object observables if needed
    public override Observable<object> Gained => gainedSubject.Select(e => (object)e);
    public override Observable<object> Lost => lostSubject.Select(e => (object)e);

    // Optional: strongly typed list
    public List<IEntity> PassingEntities => passingSet;*/

    IDisposable subscriptionBag;

    private readonly HashSet<IEntity> currentlyGainedEntities = new();

    private void Start()
    {
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
                        gainedSubject.OnNext(entity);
                        PassingSet.Add(entity);
                    }

                }
                else
                {
                    if (currentlyGainedEntities.Remove(entity))
                    {
                        gainedSubject.OnNext(entity);
                        PassingSet.Remove(entity);
                    }

                }
            });

        // Also handle entities removed from the list (unregistered)
        var disposable2 = unregisteredEntities
            .Subscribe(removeEvent =>
            {
                var entity = removeEvent.Value;

                lostSubject.OnNext(entity);

                PassingSet.Remove(entity);
            });

        subscriptionBag = Disposable.Combine(disposable1, disposable2);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

}

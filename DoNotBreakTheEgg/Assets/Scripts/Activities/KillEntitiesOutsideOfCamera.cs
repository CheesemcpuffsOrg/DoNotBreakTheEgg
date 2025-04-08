using ObservableCollections;
using R3;
using UnityEngine;

public class KillEntitiesOutsideOfCamera : MonoBehaviour
{
    [SerializeField] RespawnEntities respawnEntities;
    [SerializeField] TagFilter filter;
    [SerializeField] float viewportOffset = 0.05f;

    ObservableList<IEntity> trackedEntities = new ObservableList<IEntity>();

    bool startCalled;

    Subject<IEntity> respawnEntity = new Subject<IEntity>();

    private void Start()
    {
        var registeredEntities = EntityRegistry.RegisteredEntities();

        foreach (var entity in registeredEntities)
        {
            StartTrackingEntity(entity);
        }

        OnStartOrEnable();

        startCalled = true;

        var subscriptionBag = Disposable.CreateBuilder();

        trackedEntities
            .ObserveAdd()
            .SelectMany(entity =>
            {
                return Observable
                    .EveryUpdate()
                    .Select(_ => entity.Value)
                    .TakeUntil(trackedEntities.ObserveRemove().Where(trackedEntity => trackedEntity.Value == entity.Value));
            })
            .Subscribe(entity =>
            {
                if (!CameraUtility.IsInsideViewport(Camera.main, entity.GetEntityComponent<IAnchoringComponent>().GetPosition(), viewportOffset))
                {
                    respawnEntity.OnNext(entity);
                }
            })
            .AddTo(ref subscriptionBag);

        respawnEntity
            .Subscribe(entity =>
            {
                EntityRegistry.UnregisterEntity(entity);
                respawnEntities.RespawnEntity(entity);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

    private void StartTrackingEntity(IEntity entity)
    {
        if (!entity.GetEntityComponent<TagComponent>().PassTagFilterCheck(filter) || trackedEntities.Contains(entity)) return;  

        trackedEntities.Add(entity);
    }

    private void StopTrackingEntity(IEntity entity)
    {
        if (!entity.GetEntityComponent<TagComponent>().PassTagFilterCheck(filter) && !trackedEntities.Contains(entity)) return;

        trackedEntities.Remove(entity);
    }

    private void OnStartOrEnable()
    {
        EntityRegistry.EntityRegistered += StartTrackingEntity;
        EntityRegistry.EntityUnregistered += StopTrackingEntity;
    }

    private void OnEnable()
    {
        if (!startCalled) return;

        OnStartOrEnable();
    }

    private void OnDisable()
    {
        EntityRegistry.EntityRegistered -= StartTrackingEntity;
        EntityRegistry.EntityUnregistered -= StopTrackingEntity;
    }
}

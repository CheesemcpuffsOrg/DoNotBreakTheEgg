using ObservableCollections;
using R3;
using UnityEngine;

public class KillEntitiesOutsideOfCamera : MonoBehaviour
{
    [SerializeField] RespawnEntities respawnEntities;
    [SerializeField] float viewportOffset = 0.05f;

    [SerializeField] GameObject trackingEntitySourceObj;

    [SerializeField] TagFilter heldItemFilter;

    [Header("Sound")]
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData deathSound;

    IEntitySource trackingEntitySource => trackingEntitySourceObj.GetComponent<IEntitySource>();

    private void Start()
    {
        var subscriptionBag = Disposable.CreateBuilder();

        var entityLost = new Subject<IEntity>();
        var entityGained = new Subject<IEntity>();

        trackingEntitySource
            .LostEntities
            .Subscribe(entity =>
            {
                entityLost.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);
        
        trackingEntitySource
            .Entities
            .Subscribe(entity =>
            {
                entityGained.OnNext(entity);
            })
            .AddTo(ref subscriptionBag);

        entityGained
            .SelectMany(entity =>
            {
                return Observable
                    .EveryUpdate()
                    .Select(_ => entity)
                    .TakeUntil(entityLost.Where(e => e == entity));
            })
            .Subscribe(entity =>
            {
                if (HoldEntityManager.Instance.TryGetHeldEntity(entity, out var heldEntity) && heldEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(heldItemFilter))
                    return;

                if (!CameraUtility.IsInsideViewport(Camera.main, entity.GetEntityComponent<IAnchoringComponent>().GetPosition(), viewportOffset))
                {
                    SoundStreams.instance.PlaySound(deathSound);
                    respawnEntities.RespawnEntity(entity);
                }
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

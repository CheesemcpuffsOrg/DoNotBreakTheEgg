using ObservableCollections;
using R3;
using UnityEngine;

public class KillEntitiesOutsideOfCamera : MonoBehaviour
{
    [SerializeField] RespawnEntities respawnEntities;
    [SerializeField] float viewportOffset = 0.05f;

    [SerializeField] GameObject trackingEntitySourceObj;
    [SerializeField] GameObject respawnEntitySourceObj;

    [Header("Sound")]
    [SerializeField] SoundData deathSound;

    IEntitySource trackingEntitySource => trackingEntitySourceObj.GetComponent<IEntitySource>();

    IEntitySource respawnEntitySource => respawnEntitySourceObj.GetComponent<IEntitySource>();

    private void Start()
    {
        var subscriptionBag = Disposable.CreateBuilder();

        trackingEntitySource
            .Entities
            .SelectMany(entity =>
            {
                return Observable
                    .EveryUpdate()
                    .Select(_ => entity)
                    .TakeUntil(respawnEntitySource.Entities.Where(e => e == entity));
            })
            .Subscribe(entity =>
            {
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

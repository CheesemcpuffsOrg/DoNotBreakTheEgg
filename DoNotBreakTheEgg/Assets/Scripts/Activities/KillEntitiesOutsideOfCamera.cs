using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEntitiesOutsideOfCamera : MonoBehaviour
{
    [SerializeField] RespawnEntities respawnEntities;
    [SerializeField] TagFilter filter;

    List<IEntity> entitiesToTrack = new List<IEntity>();

    bool startCalled;


    private void Start()
    {
        var registeredEntities = EntityRegistry.RegisteredEntities();

        foreach (var entity in registeredEntities)
        {
            StartTrackingEntity(entity);
        }

        OnStartOrEnable();

        startCalled = true;
    }

    private void Update()
    {
        for (int i = entitiesToTrack.Count - 1; i >= 0; i--)
        {
            var entity = entitiesToTrack[i];
            if (!CameraUtility.IsInsideViewport(Camera.main, entity.GetEntityComponent<IAnchoringComponent>().GetPosition(), 0.1f))
            {
                EntityRegistry.UnregisterEntity(entity);
                respawnEntities.RespawnEntity(entity);
            }
        }
    }

    private void StartTrackingEntity(IEntity entity)
    {
        if (!entity.GetEntityComponent<TagComponent>().PassTagFilterCheck(filter) || entitiesToTrack.Contains(entity)) return;  

        entitiesToTrack.Add(entity);
    }

    private void StopTrackingEntity(IEntity entity)
    {
        if (!entity.GetEntityComponent<TagComponent>().PassTagFilterCheck(filter) && !entitiesToTrack.Contains(entity)) return;

        entitiesToTrack.Remove(entity);
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

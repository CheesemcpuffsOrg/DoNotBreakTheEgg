using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEntities : MonoBehaviour
{

    [SerializeField] Transform spawnPoint;

    public void RespawnEntity(IEntity entity)
    {
        EntityRegistry.RegisterEntity(entity);
        entity.GetEntityComponent<IAnchoringComponent>().SetPosition(spawnPoint.position);
    }
}

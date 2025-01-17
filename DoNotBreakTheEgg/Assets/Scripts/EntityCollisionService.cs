using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityCollisionService
{
    private static Dictionary<Collider2D, IEntity> entityColliders = new Dictionary<Collider2D, IEntity>();

    public static void RegisterEntityCollider(Collider2D collider, IEntity entity)
    {
        entityColliders.Add(collider, entity);
    }

    public static bool TryGetEntity(Collider2D collider, out IEntity entity)
    {
        if(entityColliders.TryGetValue(collider, out entity))
        {
            return true;
        }

        return false;
    }
}

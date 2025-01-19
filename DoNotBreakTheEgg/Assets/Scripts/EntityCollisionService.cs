using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static void IgnoreEntityCollisions(IEntity entity1, IEntity entity2, bool setActive)
    {
        // Find all colliders for each entity
        var entity1Colliders = entityColliders
            .Where(kvp => kvp.Value == entity1)
            .Select(kvp => kvp.Key)
            .ToList();

        var entity2Colliders = entityColliders
            .Where(kvp => kvp.Value == entity2)
            .Select(kvp => kvp.Key)
            .ToList();

        // Early exit if no colliders are found
        if (!entity1Colliders.Any() || !entity2Colliders.Any())
        {
            Debug.Log("One of the entities does not have a CollisionComponent");
            return;
        }
            

        // Ignore collisions
        foreach (var collider1 in entity1Colliders)
        {
            foreach (var collider2 in entity2Colliders)
            {
                Physics2D.IgnoreCollision(collider1, collider2, setActive);
            }
        }
    }
}

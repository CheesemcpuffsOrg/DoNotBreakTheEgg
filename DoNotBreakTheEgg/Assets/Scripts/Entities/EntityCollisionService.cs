using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EntityCollisionService
{
    private static Dictionary<Collider2D, IEntity> entityColliders = new Dictionary<Collider2D, IEntity>();

    private static Dictionary<IEntity, Collider2D> mainEntityColliders = new Dictionary<IEntity, Collider2D>();

    private static Dictionary<IEntity, List<Collider2D>> ignoredColliders = new Dictionary<IEntity, List<Collider2D>>();

    public static void RegisterEntityColliders(Collider2D[] colliders, IEntity entity)
    {
        foreach (var collider in colliders) 
        {
            if (!entityColliders.ContainsKey(collider))
            {
                entityColliders.Add(collider, entity);
            }
        }
        
    }

    public static void RegisterMainEntityCollider(Collider2D collider, IEntity entity)
    {
        if (!mainEntityColliders.ContainsKey(entity))
        {
            mainEntityColliders.Add(entity, collider);
        }
    }

    public static bool TryGetEntity(Collider2D collider, out IEntity entity)
    {
        if(entityColliders.TryGetValue(collider, out entity))
        {
            return true;
        }

        return false;
    }

    public static bool IsIgnoredCollider(IEntity referenceEntity, Collider2D collider)
    {
        if (!ignoredColliders.TryGetValue(referenceEntity, out var colliders))
        {
            return false;
        }

        foreach(var storedCollider in colliders)
        {
            if(storedCollider == collider) return true;
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

        UpdateIgnoredCollidersDictionary(entity1, entity2, setActive, entity1Colliders, entity2Colliders);

    }

    public static void IgnoreMainEntityColliders(IEntity entity1, IEntity entity2, bool setActive)
    {
        if(!mainEntityColliders.TryGetValue(entity1, out var mainCollider1))
        {
            Debug.Log(entity1 + " has not registered their main collider");
            return;
        }

        if (!mainEntityColliders.TryGetValue(entity2, out var mainCollider2))
        {
            Debug.Log(entity2 + " has not registered their main collider");
            return;
        }

        Physics2D.IgnoreCollision(mainCollider1, mainCollider2, setActive);
    }

    private static void UpdateIgnoredCollidersDictionary(IEntity entity1, IEntity entity2, bool setActive, List<Collider2D> entity1Colliders, List<Collider2D> entity2Colliders)
    {
        if (setActive)
        {
            if (!ignoredColliders.TryGetValue(entity1, out var entity1StoredColliders))
            {
                ignoredColliders.Add(entity1, entity2Colliders);
            }
            else
            {
                foreach (var collider in entity2Colliders)
                {
                    if (!entity1StoredColliders.Contains(collider))
                    {
                        entity1StoredColliders.Add(collider);
                    }
                }
            }

            if (!ignoredColliders.TryGetValue(entity2, out var entity2StoredColliders))
            {
                ignoredColliders.Add(entity2, entity1Colliders);
            }
            else
            {
                foreach (var collider in entity1Colliders)
                {
                    if (!entity2StoredColliders.Contains(collider))
                    {
                        entity2StoredColliders.Add(collider);
                    }     
                }
            }

        }
        else
        {
            if (ignoredColliders.TryGetValue(entity1, out var entity1StoredColliders))
            {
                for (int i = entity1StoredColliders.Count - 1; i >= 0; i--)
                {
                    foreach (var collider in entity2Colliders)
                    {
                        if (entity1StoredColliders[i] == collider)
                        {
                            entity1StoredColliders.RemoveAt(i);
                            break; // Exit the inner loop once removed
                        }
                    }
                }
            }

            if (ignoredColliders.TryGetValue(entity2, out var entity2StoredColliders))
            {
                for (int i = entity2StoredColliders.Count - 1; i >= 0; i--)
                {
                    foreach (var collider in entity1Colliders)
                    {
                        if (entity2StoredColliders[i] == collider)
                        {
                            entity2StoredColliders.RemoveAt(i);
                            break; // Exit the inner loop once removed
                        }
                    }
                }
            }
        }
    }
}

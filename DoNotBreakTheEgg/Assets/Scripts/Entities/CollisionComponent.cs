using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour, ICollisionComponent
{
    [SerializeField] Collider2D mainEntityCollider;
    [SerializeField] List<Collider2D> allEntityColliders;

    IEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<IEntity>();

        foreach (var collider in allEntityColliders)
        {
            EntityCollisionService.RegisterEntityCollider(collider, entity);
        }
    }

    public bool IsEntityCollider(Collider2D collider)
    {
        return allEntityColliders.Contains(collider);
    }

    public Bounds GetMainPlayerColliderBounds()
    {
        return mainEntityCollider.bounds;
    }
}

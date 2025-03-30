using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour, ICollisionComponent
{
    [SerializeField] Collider2D mainEntityCollider;
    [SerializeField] List<Collider2D> allEntityColliders;

    IEntity entity;

    private void Awake()
    {
        entity = GetComponent<IEntity>();

        EntityCollisionService.RegisterEntityColliders(allEntityColliders.ToArray(), entity);   
        EntityCollisionService.RegisterMainEntityCollider(mainEntityCollider, entity);
    }

    public bool IsEntityCollider(Collider2D collider)
    {
        return allEntityColliders.Contains(collider);
    }

    public Bounds GetEntityMainColliderBounds()
    {
        return mainEntityCollider.bounds;
    }
}

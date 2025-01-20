using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableComponent : MonoBehaviour, IBreakableComponent
{
    [Header("Collisions")]
    [SerializeField] CollisionProxy collision;

    IEntity entity;

    private void Start()
    {
        entity = GetComponent<IEntity>();
    }

    private void CollisionEnter(Collision2D collision)
    {
        if (EntityCollisionService.TryGetEntity(collision.collider, out IEntity collisionEntity))
            return;

        ThrowEntityManager.Instance.RemoveThrownEntity(entity);

        entity.GetEntityComponent<IGameObjectComponent>().Destroy(1);
    }

    private void OnEnable()
    {
        collision.OnCollisionEnter2D_Action += CollisionEnter;
    }

    private void OnDisable()
    {
        collision.OnCollisionEnter2D_Action -= CollisionEnter;
    }
}

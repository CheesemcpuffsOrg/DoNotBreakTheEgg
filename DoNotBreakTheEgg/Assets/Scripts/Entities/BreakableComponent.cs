using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableComponent : MonoBehaviour, IBreakableComponent
{
    [Header("Collisions")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isDeadTag;
    [SerializeField] TagFilter breakFilter;
    [SerializeField] TagFilter ignoreColisionsFilter;

    IEntity entity;

    private void Start()
    {
        entity = GetComponent<IEntity>();

        /*foreach (var registeredEntity in EntityRegistry.RegisteredEntities())
        {
            if (registeredEntity == null || !registeredEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(ignoreColisionsFilter)) continue;

            IgnoreMainColliderCollisions(registeredEntity);
        }*/
    }

    private void CollisionEnter(Collision2D collision)
    {
        if(!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(breakFilter)) return;

        //entity.GetEntityComponent<IMovementComponent>().DisableGravity();

        entity.GetEntityComponent<ITagComponent>().AddTag(isDeadTag);

        entity.Destroy(1);
    }

    private void TriggerEnter(Collider2D collider)
    {
        if (!EntityCollisionService.TryGetEntity(collider, out var collisionEntity)) return;

        if(!collisionEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(ignoreColisionsFilter)) return;

        IgnoreMainColliderCollisions(collisionEntity);
    }

    private void IgnoreMainColliderCollisions(IEntity ignoreEntity)
    {
        if (!ignoreEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(ignoreColisionsFilter)) return;

        //Debug.Log("Ignore collisions between the main collider of " + entity + " and " + ignoreEntity);

        EntityCollisionService.IgnoreMainEntityColliders(entity, ignoreEntity, true);
    }

    private void OnEnable()
    {
        collision.OnCollisionEnter2D_Action += CollisionEnter;
        collision.OnTriggerEnter2D_Action += TriggerEnter;
        //EntityRegistry.EntityRegistered += IgnoreMainColliderCollisions;
    }

    private void OnDisable()
    {
        collision.OnCollisionEnter2D_Action -= CollisionEnter;
        collision.OnTriggerEnter2D_Action -= TriggerEnter;
       // EntityRegistry.EntityRegistered -= IgnoreMainColliderCollisions;
    }
}

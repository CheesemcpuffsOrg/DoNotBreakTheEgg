using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionComponent : MonoBehaviour, IInteractionComponent
{

    List<IEntity> interactableEntities = new();

    [Header("Collision Proxies")]
    [SerializeField] CollisionProxy collision;

    IEntity entity;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    public void Interact()
    {
        Debug.Log("InteractCalled");

        var firstEntity = interactableEntities.TryGet(0);

        if (firstEntity == null) return;

        var closestEntity = firstEntity;
        var smallestDistance = Vector3.Distance(transform.position, firstEntity.GetEntityComponent<IAnchoringComponent>().GetPosition());

        foreach (var entity in interactableEntities)
        {
            var distance = Vector3.Distance(transform.position, entity.GetEntityComponent<IAnchoringComponent>().GetPosition());   

            if(distance < smallestDistance)
            {
                closestEntity = entity;
                smallestDistance = distance;
            }
        }

        closestEntity.GetEntityComponent<IInteractableComponent>().Interact(entity);
    }

    private void TriggerEnter(Collider2D collision)
    {
        if (!EntityCollisionService.TryGetEntity(collision, out IEntity collisionEntity)
            || interactableEntities.Contains(collisionEntity)
            || collisionEntity.GetEntityComponent<IInteractableComponent>() != null)
            return;

        interactableEntities.Add(collisionEntity);
    }

    private void TriggerExit(Collider2D collision)
    {
        if (!EntityCollisionService.TryGetEntity(collision, out IEntity collisionEntity)
            || !interactableEntities.Contains(collisionEntity))
            return;

        interactableEntities.Remove(collisionEntity);
    }

    private void OnEnable()
    {
        collision.OnTriggerEnter2D_Action += TriggerEnter;
        collision.OnTriggerExit2D_Action += TriggerExit;
    }

    private void OnDisable()
    {
        collision.OnTriggerEnter2D_Action -= TriggerEnter;
        collision.OnTriggerExit2D_Action -= TriggerExit;
    }


}

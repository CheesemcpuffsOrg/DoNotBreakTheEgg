using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionComponent : MonoBehaviour, IInteractionComponent
{

    List<IEntity> interactableEntities = new();

    [Header("Collision Proxies")]
    [SerializeField] CollisionProxy interactCollider;

    [Header("Tags")]
    [SerializeField] TagFilter interactFilter;

    float bufferTimer = 0;

    IEntity entity;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    private void Update()
    {
        bufferTimer -= Time.deltaTime;

        if(bufferTimer <= 0)
        {
            bufferTimer = 0;
        }
    }

    public void Interact()
    {
        bufferTimer = 0.2f;

        var firstEntity = interactableEntities.TryGet(0);

        if (firstEntity == null) return;

        var closestEntity = CheckDistance(firstEntity);

        //Debug.Log("Attempt to interact");

        closestEntity.GetEntityComponent<IInteractableComponent>().Interact(entity);
    }

    private IEntity CheckDistance(IEntity firstEntity)
    {
        var closestEntity = firstEntity;
        var smallestDistance = Vector3.Distance(transform.position, firstEntity.GetEntityComponent<IAnchoringComponent>().GetPosition());

        foreach (var entity in interactableEntities)
        {
            var distance = Vector3.Distance(transform.position, entity.GetEntityComponent<IAnchoringComponent>().GetPosition());

            if (distance < smallestDistance)
            {
                closestEntity = entity;
                smallestDistance = distance;
            }
        }

        return closestEntity;
    }

    private void InteractColliderTriggerEnter(Collider2D collision)
    {
        if (!EntityCollisionService.TryGetEntity(collision, out IEntity collisionEntity)
            || interactableEntities.Contains(collisionEntity)
            || collisionEntity.GetEntityComponent<IInteractableComponent>() == null
            || !collisionEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(interactFilter))
            return;

       // Debug.Log("Add to list");

        interactableEntities.Add(collisionEntity);

        if (bufferTimer <= 0) return;

        Interact();
    }

    private void InteractColliderTriggerExit(Collider2D collision)
    {

        if (!EntityCollisionService.TryGetEntity(collision, out IEntity collisionEntity)
            || !interactableEntities.Contains(collisionEntity))
            return;

       // Debug.Log("remove from list");

        interactableEntities.Remove(collisionEntity);
    }

    private void OnEnable()
    {
        interactCollider.OnTriggerEnter2D_Action += InteractColliderTriggerEnter;
        interactCollider.OnTriggerExit2D_Action += InteractColliderTriggerExit;
    }

    private void OnDisable()
    {
        interactCollider.OnTriggerEnter2D_Action -= InteractColliderTriggerEnter;
        interactCollider.OnTriggerExit2D_Action -= InteractColliderTriggerExit;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableComponent : MonoBehaviour, IHoldableComponent, IInteractableComponent
{

    [SerializeField] AnchorScriptableObject holdAnchor;

    IEntity entity;
    IMovementComponent movementComponent;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
        movementComponent = GetComponent<MovementComponent>();
    }


    public void Hold(Transform anchor)
    {
        movementComponent.DisableMovement();

        transform.position = anchor.position;
        transform.SetParent(anchor);
    }

    public void Release()
    {
        transform.SetParent (null);
        movementComponent.EnableMovement();
    }

    public void Interact(IEntity interactingEntity)
    {
        if (HoldEntityManager.Instance.IsEntityHolding(entity)) return;

        HoldEntityManager.Instance.AddHeldEntity(interactingEntity, entity, interactingEntity.GetEntityComponent<AnchoringComponent>().GetAnchor(holdAnchor));
    }
}

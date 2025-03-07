using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableComponent : MonoBehaviour, IHoldableComponent, IInteractableComponent
{

    [SerializeField] AnchorScriptableObject holdAnchor;

    Rigidbody2D rb;
    IEntity entity;
    IMovementComponent movementComponent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<IEntity>();
        movementComponent = GetComponent<MovementComponent>();

    }


    public void Hold(Transform anchor)
    {
        movementComponent.DisableMovement();

        transform.position = anchor.position;
        transform.SetParent(anchor);

        /*// Stop the object's movement and set it to Kinematic
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f; // Also reset any angular velocity (spinning)*/
    }

    public void Release()
    {
        transform.SetParent (null);
        movementComponent.EnableMovement();
    }

    public void Interact(IEntity interactingEntity)
    {
        HoldEntityManager.Instance.AddHeldEntity(interactingEntity, entity, interactingEntity.GetEntityComponent<AnchoringComponent>().GetAnchor(holdAnchor));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableComponent : MonoBehaviour, IHoldableComponent, IInteractableComponent
{

    [SerializeField] AnchorScriptableObject holdAnchor;
    [SerializeField] SpriteRenderer defaultView;
    [SerializeField] SpriteRenderer heldView;


    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData pickUpSound;

    IEntity entity;
    IMovementComponent movementComponent;
    IEntitySoundComponent soundComponent;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    private void Start()
    {
        movementComponent = entity.GetEntityComponent<IMovementComponent>();
        soundComponent = entity.GetEntityComponent<IEntitySoundComponent>();
        heldView.enabled = false;
    }

    //update this at some point to have a held anchor?

    public void Hold(Transform anchor)
    {
        movementComponent.DisableMovement();

        transform.position = anchor.position;
        transform.SetParent(anchor);
        heldView.enabled = true;
        defaultView.enabled = false;
    }

    public void Release()
    {
        transform.SetParent (null);
        movementComponent.EnableMovement();
        heldView.enabled = false;
        defaultView.enabled = true;    
    }

    public void Interact(IEntity interactingEntity)
    {
        if (HoldEntityManager.Instance.IsEntityHolding(entity) || HoldEntityManager.Instance.IsEntityHeld(interactingEntity)) return; //added in check to make sure aheld entity cannot hold, it was causing too many bugs

        soundComponent.PlaySound(pickUpSound);

        HoldEntityManager.Instance.AddHeldEntity(interactingEntity, entity, interactingEntity.GetEntityComponent<IAnchoringComponent>().GetAnchor(holdAnchor));
    }
}

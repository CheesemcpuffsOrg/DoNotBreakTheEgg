using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableComponent : MonoBehaviour, IHoldableComponent, IInteractableComponent
{

    [SerializeField] AnchorScriptableObject holdAnchor;


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

        soundComponent.PlaySound(pickUpSound);

        HoldEntityManager.Instance.AddHeldEntity(interactingEntity, entity, interactingEntity.GetEntityComponent<AnchoringComponent>().GetAnchor(holdAnchor));
    }
}

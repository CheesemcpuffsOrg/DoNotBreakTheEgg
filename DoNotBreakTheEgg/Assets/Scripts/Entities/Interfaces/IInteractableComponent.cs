using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableComponent : IEntityComponent
{
    public void Interact(IEntity interactingEntity);
}

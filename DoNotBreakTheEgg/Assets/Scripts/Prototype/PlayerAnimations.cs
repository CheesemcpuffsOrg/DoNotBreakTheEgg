using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    IEntity entity;
    IMovementComponent movementComponent;

    private void Start()
    {
        entity = GetComponent<IEntity>();
        movementComponent = entity.GetEntityComponent<IMovementComponent>();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{

    IEntity entity;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    public bool CanThrow()
    {
        return HoldingManager.Instance.IsPlayerHoldingEntity(entity);
    }

    public bool CanJump()
    {
        return HoldingManager.Instance.IsPlayerHoldingEntity(entity);
    }

    public bool CanCatch()
    {
        return !HoldingManager.Instance.IsPlayerHoldingEntity(entity);
    }
}

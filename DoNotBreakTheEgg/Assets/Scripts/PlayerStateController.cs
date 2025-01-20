using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{

    IEntity entity;

    bool isThrown;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    public bool CanThrow()
    {
        return HoldEntityManager.Instance.IsEntityHolding(entity) && !isThrown;
    }

    public bool CanJump()
    {
        return !HoldEntityManager.Instance.IsEntityHolding(entity) && !isThrown;
    }

    public void IsThrown(bool isThrown)
    {
        this.isThrown = isThrown;
    }
}

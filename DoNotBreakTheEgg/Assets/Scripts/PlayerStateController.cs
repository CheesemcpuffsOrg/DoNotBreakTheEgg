using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public bool CanThrow()
    {
        return HoldingManager.Instance.IsPlayerHoldingObject(gameObject);
    }

    public bool CanJump()
    {
        return HoldingManager.Instance.IsPlayerHoldingObject(gameObject);
    }
}

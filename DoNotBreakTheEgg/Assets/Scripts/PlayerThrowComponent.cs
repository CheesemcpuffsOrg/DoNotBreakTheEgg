using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowComponent : MonoBehaviour
{
    [SerializeField] PlayerInputController controller;

    [SerializeField] Transform launchPoint;

    [SerializeField] float powerBase = 1f;
    [SerializeField] float powerMax = 10f;
    [SerializeField] float powerIncrease = 0.02f;
    float powerCurrent;

    bool chargingShot;

    private void Update()
    {
        ChargeShot();
    }

    public void ChargeThrow()
    {
        powerCurrent = powerBase; // Reset power to the base value
        chargingShot = true; // Start charging
    }

    public void Throw()
    {
        var heldObject = HoldingManager.Instance.GetHeldObject(gameObject);
        heldObject.GetComponent<IThrowable>().Throw(powerCurrent, launchPoint);    
        HoldingManager.Instance.RemoveHeldObject(gameObject);
        chargingShot = false;
    }

    private void ChargeShot()
    {
        if (!chargingShot)
        {
            return;
        }

        powerCurrent += powerIncrease;

        // Cap the power at powerMax
        if (powerCurrent >= powerMax)
        {
            powerCurrent = powerMax;
        }
    }

    private void OnEnable()
    {
        controller.ThrowEventStarted += ChargeThrow;
        controller.ThrowEventPerformed += Throw;
    }

    private void OnDisable()
    {
        controller.ThrowEventStarted -= ChargeThrow;
        controller.ThrowEventPerformed -= Throw;
    }
}

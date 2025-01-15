using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] PlayerHolding playerHolding;
    [SerializeField] InputController controller;

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
        var objectToThrow = playerHolding.HeldObject;

        if (objectToThrow == null) return;

        objectToThrow.GetComponent<Rigidbody2D>().velocity = powerCurrent * launchPoint.up;
        objectToThrow.transform.SetParent(null);
        playerHolding.ObjectThrown();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAndCatchComponent : MonoBehaviour, IThrowAndCatchComponent
{
    [SerializeField] PlayerInputController controller;
    [SerializeField] PlayerStateController stateController;

    [SerializeField] Transform holdAnchor;

    [SerializeField] Transform launchPoint;

    [SerializeField] float powerBase = 1f;
    [SerializeField] float powerMax = 10f;
    [SerializeField] float powerIncrease = 0.02f;

    [Header("Collision Proxies")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject holdableTag;

    IEntity entity;

    float powerCurrent;

    bool chargingShot;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    private void Start()
    {
        collision.OnTriggerEnter2D_Action += TriggerEnter;
    }

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
        chargingShot = false;

        if (!stateController.CanThrow())
            return;

        var heldEntity = HoldingManager.Instance.GetHeldEntity(entity);

        heldEntity.GetEntityComponent<IThrowableComponent>().Throw(powerCurrent, launchPoint);
        HoldingManager.Instance.RemoveHeldEntity(entity);
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

    private void TriggerEnter(Collider2D collision)
    {
        if (!EntityCollisionService.TryGetEntity(collision, out IEntity collisionEntity) 
            || !stateController.CanCatch() 
            || !collisionEntity.GetEntityComponent<ITagComponent>().HasTag(holdableTag))
            return;

        HoldingManager.Instance.AddHeldEntity(entity, collisionEntity, holdAnchor);
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

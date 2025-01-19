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
    [SerializeField] TagScriptableObject[] requiredTags;

    [Header("Draw Trajectory Gizmo")]
    [SerializeField] private float entityWeight = 1f;
    [SerializeField] private int trajectorySteps = 10; // Number of points to simulate for the trajectory
    [SerializeField] private float timeStep = 0.1f;

    IEntity entity;

    float powerCurrent;

    bool chargingShot;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
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

        var heldEntity = HoldEntityManager.Instance.GetHeldEntity(entity);

        //heldEntity.GetEntityComponent<IThrowableComponent>().Throw(powerCurrent, launchPoint);
        
        HoldEntityManager.Instance.RemoveHeldEntity(entity);

        var velocity = powerCurrent * (Vector2)launchPoint.up;

        ThrowEntityManager.Instance.AddEntityToThrow(heldEntity, new ThrowEntityManager.ThrowData(velocity, -9.81f, heldEntity.GetEntityComponent<IGameObjectComponent>().GetTransform()));
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
            || collisionEntity.GetEntityComponent<ITagComponent>()?.HasAllTags(requiredTags) != true)
            return;

        if(ThrowEntityManager.Instance.IsEntityBeingThrown(collisionEntity))
            ThrowEntityManager.Instance.RemoveThrownEntity(collisionEntity);
            

        HoldEntityManager.Instance.AddHeldEntity(entity, collisionEntity, holdAnchor);
    }

    private void OnEnable()
    {
        controller.ThrowEventStarted += ChargeThrow;
        controller.ThrowEventPerformed += Throw;
        collision.OnTriggerEnter2D_Action += TriggerEnter;
    }

    private void OnDisable()
    {
        controller.ThrowEventStarted -= ChargeThrow;
        controller.ThrowEventPerformed -= Throw;
        collision.OnTriggerEnter2D_Action -= TriggerEnter;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (chargingShot && launchPoint != null)
        {
            Gizmos.color = Color.red;
            DrawTrajectory(launchPoint.position, powerCurrent);
        }
    }

    private void DrawTrajectory(Vector3 startPosition, float power)
    {
        // Calculate initial velocity based on power and entity weight
        Vector3 velocity = (power / entityWeight) * launchPoint.up;

        // Get the Rigidbody2D's gravity scale and mass
        float gravity = Physics2D.gravity.y * 1;

        // Simulate the trajectory
        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < trajectorySteps; i++)
        {
            // Apply gravity to the vertical velocity over time based on Rigidbody2D physics
            currentVelocity.y += gravity * timeStep;

            // Update position based on velocity
            currentPosition += currentVelocity * timeStep;

            // Draw spheres at each point
            Gizmos.DrawSphere(currentPosition, 0.1f);

            if (i > 0)
            {
                // Draw a line from the previous point to the current point
                Gizmos.DrawLine(currentPosition, currentPosition - currentVelocity * timeStep);
            }
        }
    }

#endif 
}

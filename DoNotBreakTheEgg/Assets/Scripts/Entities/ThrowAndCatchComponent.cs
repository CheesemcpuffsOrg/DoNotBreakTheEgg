using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThrowAndCatchComponent : MonoBehaviour, IThrowAndCatchComponent
{
    [SerializeField] Transform holdAnchor;

    [SerializeField] Transform launchPoint;

    [SerializeField] float powerBase = 1f;
    [SerializeField] float powerMax = 10f;
    [SerializeField] float powerIncrease = 0.02f;

    [Header("Collision Proxies")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isHeldTag;
    [SerializeField] TagScriptableObject isHoldingTag;
    [SerializeField] TagFilter catchableEntityFilter;
    [SerializeField] TagFilter catchingFilter;
    [SerializeField] TagFilter throwFilter;


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

        if (!throwFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>().GetTransform()))
            return;

        var heldEntity = HoldEntityManager.Instance.GetHeldEntity(entity);
        
        HoldEntityManager.Instance.RemoveHeldEntity(entity);

        heldEntity.GetEntityComponent<IMovementComponent>().Throw(powerCurrent, (Vector2)launchPoint.up);
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
            || !catchableEntityFilter.PassTagFilterCheck(collisionEntity.GetEntityComponent<IGameObjectComponent>()?.GetTransform())
            || !catchingFilter.PassTagFilterCheck(entity.GetEntityComponent<IGameObjectComponent>()?.GetTransform()))
            return;
            
        HoldEntityManager.Instance.AddHeldEntity(entity, collisionEntity, holdAnchor);
    }

    private void OnEnable()
    {
        collision.OnTriggerEnter2D_Action += TriggerEnter;
    }

    private void OnDisable()
    {
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

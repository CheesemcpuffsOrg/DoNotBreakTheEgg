using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingManager : MonoBehaviour
{

    public static HoldingManager Instance;

    Dictionary<IEntity, IEntity> heldObjects = new Dictionary<IEntity, IEntity>();

    private void Awake()
    {
        // Check if there's already an instance of this class
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Another instance of the singleton {nameof(HoldingManager)} exists. Please make sure there is only one");
            return;
        }

        Instance = this;
    }

    public bool IsPlayerHoldingEntity(IEntity holdingEntity)
    {
        return heldObjects.TryGetValue(holdingEntity, out var heldEntity) && heldEntity != null;
    }

    public IEntity GetHeldEntity(IEntity holdingEntity)
    {
        if (!heldObjects.TryGetValue(holdingEntity, out var heldEntity))
            return null;

        return heldEntity;
    }

    public void AddHeldEntity(IEntity holdingEntity, IEntity heldEntity, Transform holdAnchor)
    {
        if(heldObjects.ContainsValue(heldEntity))
            return;

        if (heldObjects.ContainsKey(holdingEntity))
        {
            heldObjects[holdingEntity] = heldEntity;
        }
        else
        {
            heldObjects.Add(holdingEntity, heldEntity);
        }

        var gameObjectComponent = heldEntity.GetEntityComponent<IGameObjectComponent>();
        gameObjectComponent.SetPosition(holdAnchor.position);
        gameObjectComponent.SetParent(holdAnchor);

        Rigidbody2D rb = gameObjectComponent.GetRigibody();

        // Stop the object's movement and set it to Kinematic
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f; // Also reset any angular velocity (spinning)
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void RemoveHeldEntity(IEntity holdingEntity)
    {
        if(!heldObjects.TryGetValue(holdingEntity, out var heldEntity))
            return;

        var gameObjectComponent = heldEntity.GetEntityComponent<IGameObjectComponent>();

        gameObjectComponent.SetParent(null);
        heldObjects[holdingEntity] = null;
        gameObjectComponent.GetRigibody().bodyType = RigidbodyType2D.Dynamic;
    }
}

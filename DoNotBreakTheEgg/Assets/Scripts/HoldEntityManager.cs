using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldEntityManager : MonoBehaviour
{

    public static HoldEntityManager Instance;

    Dictionary<IEntity, IEntity> heldObjects = new Dictionary<IEntity, IEntity>();

    private void Awake()
    {
        // Check if there's already an instance of this class
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Another instance of the singleton {nameof(HoldEntityManager)} exists. Please make sure there is only one");
            return;
        }

        Instance = this;
    }

    public bool IsEntityHolding(IEntity holdingEntity)
    {
        return heldObjects.TryGetValue(holdingEntity, out var heldEntity) && heldEntity != null;
    }

    public bool IsEntityHeld(IEntity entity)
    {
        return heldObjects.ContainsValue(entity);
    }

    public IEntity GetHeldEntity(IEntity holdingEntity)
    {
        if (!heldObjects.TryGetValue(holdingEntity, out var heldEntity))
            return null;

        return heldEntity;
    }

    public void AddHeldEntity(IEntity holdingEntity, IEntity heldEntity, Transform holdAnchor)
    {
        if(heldObjects.ContainsValue(heldEntity) || IsEntityHolding(holdingEntity))
            return;

        if (heldObjects.ContainsKey(holdingEntity))
        {
            heldObjects[holdingEntity] = heldEntity;
        }
        else
        {
            heldObjects.Add(holdingEntity, heldEntity);
        }

        heldEntity.GetEntityComponent<IHoldableComponent>().Hold(holdAnchor);

    }

    public void RemoveHeldEntity(IEntity holdingEntity)
    {
        if(!heldObjects.TryGetValue(holdingEntity, out var heldEntity))
            return;

        heldObjects[holdingEntity] = null;

        heldEntity.GetEntityComponent<IHoldableComponent>().Release();

        
    }
}

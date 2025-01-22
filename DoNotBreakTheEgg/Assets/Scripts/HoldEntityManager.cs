using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldEntityManager : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] TagScriptableObject isHeldTag;
    [SerializeField] TagScriptableObject isHoldingTag;

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
        if (heldObjects.ContainsValue(heldEntity) || IsEntityHolding(holdingEntity) || holdingEntity == heldEntity)
            return;

        if (heldObjects.ContainsKey(holdingEntity))
        {
            heldObjects[holdingEntity] = heldEntity;
        }
        else
        {
            heldObjects.Add(holdingEntity, heldEntity);
        }

        // EntityCollisionService.IgnoreEntityCollisions(heldEntity, holdingEntity, true);

        heldEntity.GetEntityComponent<ITagComponent>().AddTag(isHeldTag);
        holdingEntity.GetEntityComponent<ITagComponent>().AddTag(isHoldingTag);

        heldEntity.GetEntityComponent<IHoldableComponent>().Hold(holdAnchor);
    }

    public void RemoveHeldEntity(IEntity holdingEntity)
    {
        if(!heldObjects.TryGetValue(holdingEntity, out var heldEntity))
            return;

        heldObjects.Remove(holdingEntity);

        heldEntity.GetEntityComponent<IHoldableComponent>().Release();

        heldEntity.GetEntityComponent<ITagComponent>().RemoveTag(isHeldTag);
        holdingEntity.GetEntityComponent<ITagComponent>().RemoveTag(isHoldingTag);

        StartCoroutine(EnableEntityCollisions(heldEntity, holdingEntity));
    }

    IEnumerator EnableEntityCollisions(IEntity heldEntity, IEntity holdingEntity)
    {
        yield return new WaitForSeconds(.5f);

       // EntityCollisionService.IgnoreEntityCollisions(heldEntity, holdingEntity, false);
    }
}

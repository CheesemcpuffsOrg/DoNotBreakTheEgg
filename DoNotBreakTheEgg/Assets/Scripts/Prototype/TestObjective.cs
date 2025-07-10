using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjective : MonoBehaviour
{

    [SerializeField] GameObject winnerText;

    [Header("Collisions")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject eggTag;

    private void TriggerEnter(Collider2D collision)
    {
        if (!EntityCollisionService.TryGetEntity(collision, out var entity)) 
            return;

        if (!HoldEntityManager.Instance.TryGetHeldEntity(entity, out var heldEntity))
            return;

        if (!heldEntity.GetEntityComponent<TagComponent>().HasTag(eggTag))
            return;

        winnerText.SetActive(true);
    }

    private void OnEnable()
    {
        collision.OnTriggerEnter2D_Action += TriggerEnter;
    }

    private void OnDisable()
    {
        collision.OnTriggerEnter2D_Action -= TriggerEnter;
    }
}

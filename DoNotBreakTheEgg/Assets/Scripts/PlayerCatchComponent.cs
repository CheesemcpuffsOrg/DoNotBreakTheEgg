using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchComponent : MonoBehaviour
{
    [SerializeField] PlayerStateController playerStateController;

    [SerializeField] Transform holdAnchor;

    [Header("Collision Proxies")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject holdableTag;

    private void Start()
    {
        collision.OnTriggerEnter2D_Action += TriggerEnter;
    }

    private void TriggerEnter(Collider2D collision)
    {
        if(collision.gameObject.HasTag(holdableTag) && playerStateController.CanCatch())
        {
            HoldingManager.Instance.AddHeldObject(gameObject, collision.gameObject.transform.root, holdAnchor);
        }
    }
}

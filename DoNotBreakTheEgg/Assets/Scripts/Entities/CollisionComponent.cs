using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour, ICollisionComponent
{

    [SerializeField] Collider2D collider;

    IEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<IEntity>();

        EntityCollisionService.RegisterEntityCollider(collider, entity);
    }
}

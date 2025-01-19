using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour, ICollisionComponent
{

    [SerializeField] List<Collider2D> colliders;

    IEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<IEntity>();

        foreach (var collider in colliders)
        {
            EntityCollisionService.RegisterEntityCollider(collider, entity);
        }
    }
}

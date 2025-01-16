using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableComponent : MonoBehaviour, IThrowableComponent
{

    [SerializeField, Tooltip("1 is the default weight modifier"), Min(1)] float entityWeight = 1;

    IEntity entity;

    Rigidbody2D rb;

    private void Awake()
    {
        entity = gameObject.GetComponent<IEntity>();
    }
    private void Start()
    {
        rb = entity.GetEntityComponent<IGameObjectComponent>().GetRigibody();
    }

    public void Throw(float power, Transform startPoint)
    {
        rb.velocity = power * startPoint.up / entityWeight;
    }
}

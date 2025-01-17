using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableComponent : MonoBehaviour, IThrowableComponent
{

    [SerializeField, Tooltip("1 is the default weight modifier"), Min(1)] float entityWeight = 1;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Throw(float power, Transform startPoint)
    {
        rb.velocity = power * startPoint.up / entityWeight;
    }
}

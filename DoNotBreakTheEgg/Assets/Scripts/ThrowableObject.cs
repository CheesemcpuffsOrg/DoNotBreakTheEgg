using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour, IThrowable
{

    [SerializeField] Rigidbody2D rb;

    public void Throw(float power, Transform startPoint)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = power * startPoint.up;
    }
}

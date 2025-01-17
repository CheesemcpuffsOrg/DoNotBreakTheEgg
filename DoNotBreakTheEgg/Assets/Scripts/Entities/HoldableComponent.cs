using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableComponent : MonoBehaviour, IHoldableComponent
{

    IEntity entity;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        entity = GetComponent<IEntity>();
    }


    public void Hold(Transform anchor)
    {
        transform.position = anchor.position;
        transform.SetParent(anchor);

        // Stop the object's movement and set it to Kinematic
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f; // Also reset any angular velocity (spinning)
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Release()
    {
        transform.SetParent (null);
        
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

}

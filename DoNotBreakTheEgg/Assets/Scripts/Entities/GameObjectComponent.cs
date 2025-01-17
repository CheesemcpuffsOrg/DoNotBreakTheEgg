using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameObjectComponent : MonoBehaviour, IGameObjectComponent
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetPosition() 
    { 
        return transform.position; 
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position; 
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }
}

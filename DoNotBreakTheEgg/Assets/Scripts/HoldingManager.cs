using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingManager : MonoBehaviour
{

    public static HoldingManager Instance;

    Dictionary<GameObject, Transform> heldObjects = new Dictionary<GameObject, Transform>();

    private void Awake()
    {
        // Check if there's already an instance of this class
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Another instance of the singleton {nameof(HoldingManager)} exists. Please make sure there is only one");
            return;
        }

        Instance = this;
    }

    public bool IsPlayerHoldingObject(GameObject player)
    {
        return heldObjects.TryGetValue(player, out var heldObject) && heldObject != null;
    }

    public Transform GetHeldObject(GameObject player)
    {
        if (!heldObjects.TryGetValue(player, out var heldObject))
            return null;

        return heldObject;
    }

    public void AddHeldObject(GameObject player, Transform heldObject, Transform holdAnchor)
    {
        if(heldObjects.ContainsValue(heldObject))
            return;

        heldObjects.Add(player, heldObject);
        heldObject.position = holdAnchor.position;
        heldObject.SetParent(holdAnchor);

        Rigidbody2D rb = heldObject.GetComponent<Rigidbody2D>();

        // Stop the object's movement and set it to Kinematic
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f; // Also reset any angular velocity (spinning)
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void RemoveHeldObject(GameObject player)
    {
        if(!heldObjects.TryGetValue(player, out var heldObject))
            return;

        heldObject.transform.SetParent(null);
        heldObjects[player] = null;
        heldObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

    }
}

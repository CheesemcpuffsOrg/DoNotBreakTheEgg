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

    public void ThrowHeldObject(GameObject player, float power, Transform statrtPoint)
    {
        if(heldObjects.TryGetValue(player, out var heldObject))
        {
            heldObject.GetComponent<IThrowable>().Throw(power, statrtPoint);
            RemoveHeldObject(player);
            heldObject.transform.SetParent(null);
        }
    }

    public void AddHeldObject(GameObject player, Transform heldObject, Transform holdAnchor)
    {
        heldObjects.Add(player, heldObject);
        heldObject.position = holdAnchor.position;
        heldObject.SetParent(holdAnchor);
        heldObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    private void RemoveHeldObject(GameObject player)
    {
        heldObjects[player] = null;
    }
}

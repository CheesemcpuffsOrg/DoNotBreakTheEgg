using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolding : MonoBehaviour
{

    [SerializeField] GameObject heldObject;
    public GameObject HeldObject => heldObject;
    [SerializeField] Transform holdAnchor;

    // Start is called before the first frame update
    void Start()
    {
        if(heldObject != null)
        {
            heldObject.transform.SetParent(holdAnchor);
            heldObject.transform.position = holdAnchor.position;
        }
    }

    public void ObjectThrown()
    {
        heldObject = null;
    }
}

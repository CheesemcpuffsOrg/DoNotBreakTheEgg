using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMe : MonoBehaviour
{

    [SerializeField] GameObject player;

    [SerializeField] Transform objectToHold;

    [SerializeField] Transform holdAnchor;

    // Start is called before the first frame update
    void Start()
    {
        HoldingManager.Instance.AddHeldObject(player, objectToHold, holdAnchor);
    }
}

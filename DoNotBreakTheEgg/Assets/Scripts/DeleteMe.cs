using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMe : MonoBehaviour
{

    [SerializeField] MonoEntity player;

    [SerializeField] MonoEntity objectToHold;

    [SerializeField] Transform holdAnchor;

    // Start is called before the first frame update
    void Start()
    {
        HoldEntityManager.Instance.AddHeldEntity(player, objectToHold, holdAnchor);
    }
}

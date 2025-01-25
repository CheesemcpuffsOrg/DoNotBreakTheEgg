using UnityEngine;

public class DeleteMe : MonoBehaviour
{

    [SerializeField] MonoEntity player;

    [SerializeField] MonoEntity objectToHold;

    [SerializeField] AnchorScriptableObject holdAnchor;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isHeldTag;
    [SerializeField] TagScriptableObject isHoldingTag;

    // Start is called before the first frame update
    void Start()
    {
        HoldEntityManager.Instance.AddHeldEntity(player, objectToHold, player.GetComponent<IGameObjectComponent>().GetAnchor(holdAnchor));
    }
}

using UnityEngine;

public class DeleteMe : MonoBehaviour
{

    [SerializeField] MonoEntity player;

    [SerializeField] MonoEntity objectToHold;

    [SerializeField] Transform holdAnchor;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isHeldTag;
    [SerializeField] TagScriptableObject isHoldingTag;

    // Start is called before the first frame update
    void Start()
    {
        HoldEntityManager.Instance.AddHeldEntity(player, objectToHold, holdAnchor);
        objectToHold.GetEntityComponent<ITagComponent>().AddTag(isHeldTag);
        player.GetEntityComponent<ITagComponent>().AddTag(isHoldingTag);

        // Log the player's tags after adding the tag
        var playerTags = player.GetEntityComponent<ITagComponent>().GetTags();
    }
}

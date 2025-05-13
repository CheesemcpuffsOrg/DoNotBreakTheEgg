using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class ToggleJumpWithEgg : MonoBehaviour
{

    [SerializeField] TagScriptableObject playerTag;

    [SerializeField] Button toggleButton;

    // Start is called before the first frame update
    void Start()
    {

        var subscriptionBag = Disposable.CreateBuilder();

        EntityRegistry
            .RegisteredEntities
            .ObserveAdd()
            .Where(addEvent => addEvent.Value.GetEntityComponent<ITagComponent>().HasTag(playerTag))
            .SelectMany(addEvent =>
            {
                return toggleButton
                    .OnClickAsObservable()
                    .Select(_ => addEvent.Value);   
            })
            .Subscribe(entity =>
            {
                entity.GetEntityComponent<IMovementComponent>().TestToggleIgnoreJumpFilter();
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }



   
}

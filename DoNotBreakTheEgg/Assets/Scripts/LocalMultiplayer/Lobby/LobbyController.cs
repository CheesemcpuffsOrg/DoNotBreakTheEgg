using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyController : MonoBehaviour
{
    [SerializeField] string startGameActionGamepad = "start";
    [SerializeField] string startGameActionKeyboard = "enter";

    [SerializeField] SceneReferenceScriptableObject sceneReferenceScriptableObject;

    InputAction startGameAction = new InputAction(type: InputActionType.Button);

    ReactiveProperty<int> playerCount = new ReactiveProperty<int>();

    private IDisposable subscritpionBag;

    bool startCalled;

    private Action<InputActionCollectionAndUserData> _onUserCreated;
    private Action<int> _onUserDeleted;

    // Start is called before the first frame update
    void Start()
    {

        OnStartOrEnable();

        startGameAction.AddBinding($"<Gamepad>/{startGameActionGamepad}");
        startGameAction.AddBinding($"<Keyboard>/{startGameActionKeyboard}");

        startGameAction.started += StartGame;

        var disposable1 = playerCount
            .Select(count => count > 0)
            .Subscribe(enableStart =>
            {
                if (enableStart)
                {
                    startGameAction.Enable();
                }
                else
                {
                    startGameAction.Disable();
                }
            });

        subscritpionBag = Disposable.Combine(disposable1);

        subscritpionBag.RegisterTo(this.destroyCancellationToken);

        startCalled = true;
    }

    public void StartGame(InputAction.CallbackContext context)
    {        
        SceneManagerService.LoadScene(sceneReferenceScriptableObject);
    }

    public void UserCreated(InputActionCollectionAndUserData data)
    {
        playerCount.Value++;
        PlayerDataStorage.StorePlayerData(data.UserData.Id, data);
    }

    public void UserDeleted(int playerID)
    {
        playerCount.Value--;
        PlayerDataStorage.RemovePlayerData(playerID);
    }


    void OnStartOrEnable()
    {
        LocalPlayerCreationManager.Instance.UserCreated += UserCreated;
        LocalPlayerCreationManager.Instance.UserDeleted += UserDeleted;
    }

    private void OnEnable()
    {
        if (!startCalled) return;

        OnStartOrEnable();
    }

    private void OnDisable()
    {
        LocalPlayerCreationManager.Instance.UserCreated -= UserCreated;
        LocalPlayerCreationManager.Instance.UserDeleted -= UserDeleted;
    }

}

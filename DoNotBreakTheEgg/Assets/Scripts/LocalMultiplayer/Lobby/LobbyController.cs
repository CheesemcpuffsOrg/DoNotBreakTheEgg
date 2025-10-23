using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class LobbyController : MonoBehaviour
{
    [SerializeField] string startGameActionGamepad = "start";
    [SerializeField] string startGameActionKeyboard = "enter";
    [SerializeField] string leaveLobbyActionGamepad = "buttonEast";
    [SerializeField] string leaveLobbyActionKeyboard = "escape";

    [SerializeField] SceneReferenceScriptableObject sceneReferenceScriptableObject;
    [SerializeField] SceneReferenceScriptableObject mainMenuSceneReferenceScriptableObject;

    InputAction startGameAction = new InputAction(type: InputActionType.Button);
    InputAction leaveLobbyAction = new InputAction(type: InputActionType.Button);

    ReactiveProperty<int> playerCount = new ReactiveProperty<int>();

    private IDisposable subscritpionBag;

    bool startCalled;

    // Start is called before the first frame update
    void Start()
    {
        OnStartOrEnable();

        startGameAction.AddBinding($"<Gamepad>/{startGameActionGamepad}");
        startGameAction.AddBinding($"<Keyboard>/{startGameActionKeyboard}");

        leaveLobbyAction.AddBinding($"<Gamepad>/{leaveLobbyActionGamepad}");
        leaveLobbyAction.AddBinding($"<Keyboard>/{leaveLobbyActionKeyboard}");
       
        startGameAction.started += StartGame;
        leaveLobbyAction.started += LeaveLobby;

        var disposable1 = playerCount
            .Append(0)
            .Select(count => count > 0)
            .Subscribe(enableStart =>
            {
                if (enableStart)
                {
                    startGameAction.Enable();
                    leaveLobbyAction.Disable();
                }
                else
                {
                    startGameAction.Disable();
                    leaveLobbyAction.Enable();
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

    public void LeaveLobby(InputAction.CallbackContext context)
    {
        SceneManagerService.LoadScene(mainMenuSceneReferenceScriptableObject);
    }

    public void UserCreated(InputActionCollectionAndUserData data)
    {
        playerCount.Value++;
        PlayerDataStorage.Instance.AddUser(data);
    }

    public void UserDeleted(int playerID)
    {
        playerCount.Value--;
        PlayerDataStorage.Instance.RemoveUser(playerID);
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

    private void OnDestroy()
    {
        startGameAction.started -= StartGame;
        startGameAction.Disable();
        startGameAction.Dispose();

        leaveLobbyAction.started -= LeaveLobby;
        leaveLobbyAction.Disable();
        leaveLobbyAction.Dispose();  
    }

}

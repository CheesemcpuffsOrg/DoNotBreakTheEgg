using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//separate the setup for join and leave, when you join disable the join enable the leave, when you leave disable leave enable join 
public class LocalPlayerCreationManager : MonoBehaviour
{
    private class InputActions
    {
        private readonly InputAction joinAction;
        private readonly InputAction leaveAction;

        public InputActions(InputAction joinAction, InputAction leaveAction)
        {
            this.joinAction = joinAction;
            this.leaveAction = leaveAction;
        }

        public void EnableJoinAction()
        {
            joinAction.Enable();
        }

        public void EnableLeaveAction()
        {
            leaveAction.Enable();
        }

        public void DisableJoinAction()
        {
            joinAction.Disable();
        }

        public void DisableLeaveAction()
        {
            leaveAction.Disable();
        }

        public void CleanUp()
        {
            joinAction.Disable();
            joinAction.Dispose();
            leaveAction.Disable();
            leaveAction.Dispose();
        }
    }

    public static LocalPlayerCreationManager Instance;

    [SerializeField] int maxPlayers = 2;
    [SerializeField] GameObject generatedInputActionAssetObj;

    private IGeneratedInputActionAsset generatedInputActionAsset;
    private IGeneratedInputActionAsset GeneratedInputActionAsset => generatedInputActionAsset ??= generatedInputActionAssetObj.GetComponent<IGeneratedInputActionAsset>();

    [Header("Input Bindings")]
    [SerializeField] string joinActionGamepad = "<button>";
    [SerializeField] string joinActionKeyboard = "<button>";
    [SerializeField] string joinActionMouse = "<button>";
    [SerializeField] string leaveActionGamepad = "buttonEast";
    [SerializeField] string leaveActionKeyboard = "escape";

    public event Action<InputActionCollectionAndUserData> UserCreated;
    public event Action<int> UserDeleted;
    public event Action AllUsersDeleted;

    int joinedCount;

    private readonly Dictionary<InputDevice, InputActions> deviceActions = new();

    void Awake()
    {
        foreach (var device in InputSystem.devices)
        {
            SetupJoinForDevice(device);
        }

        // Also, subscribe to new device connections during runtime
        InputSystem.onDeviceChange += OnDeviceChange;

        Instance = this;
    }

    public void JoinLobby(InputDevice device)
    {
        if (UserDeviceMappingUtil.IsDevicePairedWithUser(device) || joinedCount >= maxPlayers)
            return;

        if (!UserDeviceMappingUtil.TryCreateUser(device, GeneratedInputActionAsset, out var mapping))
            return;

        UserCreated?.Invoke(mapping);

        joinedCount++;

        if (!deviceActions.TryGetValue(device, out var inputActions)) return;

        inputActions.EnableLeaveAction();
        inputActions.DisableJoinAction();
    }

    public void LeaveLobby(InputDevice device)
    {
        if(joinedCount <= 0)
        {
            AllUsersDeleted?.Invoke();
        }

        if (!UserDeviceMappingUtil.IsDevicePairedWithUser(device)) return;

        if (!UserDeviceMappingUtil.TryDeleteUser(device, out var inputUserData)) return;

        joinedCount--;

        UserDeleted?.Invoke(inputUserData.Id);

        if (!deviceActions.TryGetValue(device, out var inputActions)) return;

        inputActions.DisableLeaveAction();
        inputActions.EnableJoinAction();

        if (joinedCount > 0) return;
        
        //load main menu scene
        AllUsersDeleted?.Invoke();
    }

    public void RemoveInputActionMappings()
    {
        foreach (var device in InputSystem.devices)
        {
            RemoveDevice(device);
        }
    }

    private void SetupJoinForDevice(InputDevice device)
    {
        if (deviceActions.ContainsKey(device))
            return;

        // Define a join action for this specific device
        var joinAction = new InputAction(type: InputActionType.Button);
        var leaveAction = new InputAction(type: InputActionType.Button);

        switch (device)
        {
            case Gamepad:
                joinAction.AddBinding($"{device.path}/{joinActionGamepad}");
                leaveAction.AddBinding($"{device.path}/{leaveActionGamepad}");
                break;
            case Keyboard:
                joinAction.AddBinding($"Keyboard/{joinActionKeyboard}");
                leaveAction.AddBinding($"Keyboard/{leaveActionKeyboard}");
                break;
            case Mouse:
                joinAction.AddBinding($"Mouse/{joinActionMouse}");
                leaveAction.AddBinding($"Keyboard/{leaveActionKeyboard}");
                break;
            default:
                return;
        }

        leaveAction.started += ctx => LeaveLobby(device);
        joinAction.started += ctx => JoinLobby(device);
        joinAction.Enable();

        deviceActions.Add(device, new InputActions(joinAction, leaveAction));
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            SetupJoinForDevice(device);
        }
        else if (change == InputDeviceChange.Removed)
        {
            RemoveDevice(device);
        }
    }

    private void RemoveDevice(InputDevice device)
    {
        if (deviceActions.TryGetValue(device, out var inputActions))
        {
            inputActions.CleanUp();
            deviceActions.Remove(device);
        }
    }
}

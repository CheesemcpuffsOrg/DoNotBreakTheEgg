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

    private void JoinLobby(InputDevice device)
    {
        if (UserDeviceMappingUtil.IsDevicePairedWithUser(device) || joinedCount >= maxPlayers)
            return;

        if (!UserDeviceMappingUtil.TryCreateUser(device, GeneratedInputActionAsset, out var mapping))//factory?
            return;

        UserCreated?.Invoke(mapping);

        joinedCount++;

        if (!deviceActions.TryGetValue(device, out var inputActions)) return;

        inputActions.EnableLeaveAction();
        inputActions.DisableJoinAction();
    }

    private void LeaveLobby(InputDevice device)
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
                joinAction.AddBinding($"{device.path}/{joinActionKeyboard}");
                leaveAction.AddBinding($"{device.path}/{leaveActionKeyboard}");
                break;
            case Mouse:
                joinAction.AddBinding($"{device.path}/{joinActionMouse}");
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

    

    /*private void JoinLobby(InputAction.CallbackContext context)
    {
        var device = context.control.device;


        LoggingUtility.EditorOnlyLog("Join attempt");

        if (UserDeviceMappingUtil.IsDevicePairedWithUser(device) || joinedCount >= maxPlayers)
        {
            return;
        }

        if (!UserDeviceMappingUtil.TryCreateUser(device, GeneratedInputActionAsset, out var mapping)) return;

        UserCreated?.Invoke(mapping);

        joinedCount++;

        if(device is Keyboard || device is Mouse)
        {
            DisableJoinActionForDevice(Keyboard.current);
            DisableJoinActionForDevice(Mouse.current);
        }
        else
        {
            DisableJoinActionForDevice(device);
        }
    }*/

    /*private void LeaveLobby(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (disabledLeaveActions.Contains(device))
        {
            return;
        }

        if (joinedCount <= 0)
        {
            //load main menu scene
            AllUsersDeleted?.Invoke();
            return;
        }

        if (!UserDeviceMappingUtil.TryDeleteUser(device, out var inputUserData)) return;

        UserDeleted?.Invoke(inputUserData.Id);

        joinedCount--;

        EnableJoinActionForDevice(device);
    }*/

    /*/// <summary>
    /// Call this method to turn on the lobby functionality
    /// </summary>
    public void EnableActions()
    {
        joinAction.Enable();
        leaveAction.Enable();
    }

    /// <summary>
    /// Call this method to turn off the lobby functionality
    /// </summary>
    public void DisableActions()
    {
        joinAction.Disable();
        leaveAction.Disable();
    }

    /// <summary>
    /// Enable join action for a specific device.
    /// </summary>
    public void EnableJoinActionForDevice(InputDevice device)
    {
        if (device == null) return;

        disabledJoinActions.Remove(device);
    }

    /// <summary>
    /// Disable join action for a specific device.
    /// </summary>
    public void DisableJoinActionForDevice(InputDevice device)
    {
        if (device == null) return;

        disabledJoinActions.Add(device);
    }

    /// <summary>
    /// Enable leave action for a specific device.
    /// </summary>
    public void EnableLeaveActionForDevice(InputDevice device)
    {
        if (device == null) return;

        disabledLeaveActions.Remove(device);
    }

    /// <summary>
    /// Disable leave action for a specific device.
    /// </summary>
    public void DisableLeaveActionForDevice(InputDevice device)
    {
        if (device == null) return;

        disabledLeaveActions.Add(device);
    }

    private void OnEnable()
    {
        EnableActions();
    }

    void OnDisable()
    {
        DisableActions();
    }*/
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class LocalPlayerCreationManager : MonoBehaviour
{
    public static LocalPlayerCreationManager Instance;

    [SerializeField] int maxPlayers = 2;
    [SerializeField] GameObject generatedInputActionAssetObj;
    private IGeneratedInputActionAsset GeneratedInputActionAsset => generatedInputActionAssetObj.GetComponent<IGeneratedInputActionAsset>();

    [Header("Input Bindings")]
    [SerializeField] string joinActionGamepad = "<Gamepad>/<button>";
    [SerializeField] string joinActionKeyboard = "<Keyboard>/<button>";
    [SerializeField] string joinActionMouse = "<Mouse>/<button>";
    [SerializeField] string leaveActionGamepad = "<Gamepad>/buttonEast";
    [SerializeField] string leaveActionKeyboard = "<Keyboard>/escape";

    /*[SerializeField] string startGameActionGamepad = "<Gamepad>/buttonWest";
    [SerializeField] string startGameActionKeyboard = "<Keyboard>/space";*/

    public event Action<(IInputActionCollection2, InputUser)> UserCreated;
    public event Action<InputUser> UserDeleted;
    public event Action AllUsersDeleted;

    InputAction joinAction;
    InputAction leaveAction;

    int joinedCount;

    private readonly HashSet<InputDevice> disabledJoinActions = new HashSet<InputDevice>();
    private readonly HashSet<InputDevice> disabledLeaveActions = new HashSet<InputDevice>();

    void Awake()
    {
        Instance = this;

        // Bind joinAction
        joinAction = new InputAction(binding: joinActionGamepad);
        joinAction.AddBinding(joinActionKeyboard);
        joinAction.AddBinding(joinActionMouse);
        joinAction.started += JoinLobby;

        // Bind leaveAction
        leaveAction = new InputAction(binding: leaveActionGamepad);
        leaveAction.AddBinding(leaveActionKeyboard);
        leaveAction.started += LeaveLobby;

        EnableActions();
    }

    private void JoinLobby(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (disabledJoinActions.Contains(device) || joinedCount >= maxPlayers)
        {
            return;
        }

        if (!UserDeviceMappingUtil.TryCreateUser(device, GeneratedInputActionAsset, out var newUserInputActions)) return;

        var inputUser = InputUser.FindUserPairedToDevice(device).Value;

        UserCreated?.Invoke((newUserInputActions, inputUser));

        joinedCount++;

        DisableJoinActionForDevice(device);
    }

    private void LeaveLobby(InputAction.CallbackContext context)
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

        var inputUser = InputUser.FindUserPairedToDevice(device).Value;

        var userID = InputUser.FindUserPairedToDevice(device).Value.id;

        if (!UserDeviceMappingUtil.TryDeleteUser(device)) return;

        UserDeleted?.Invoke(inputUser);

        joinedCount--;

        EnableJoinActionForDevice(device);
    }

    /// <summary>
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

    void OnDisable()
    {
        DisableActions();
    }
}

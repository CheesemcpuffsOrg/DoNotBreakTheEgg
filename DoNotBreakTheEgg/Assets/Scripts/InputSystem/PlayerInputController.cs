using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerInputController : MonoBehaviour
{

    Controls controls;

    public event Action ThrowEventStarted;
    public event Action ThrowEventPerformed;
    public event Action<Vector2> AimEventPerformed;
    public event Action AimEventCancelled;
    public event Action<Vector2> MoveEventPerfomed;
    public event Action MoveEventCancelled;
    public event Action JumpEventPerformed;
    public event Action InteractEventPerformed;

    public void InitializeControls(IInputActionCollection2 inputActions, InputSystemUIInputModule inputSystemUIInputModule)
    {
        controls = (Controls)inputActions;

        controls.PlayerControls.Throw.started += ThrowStarted;
        controls.PlayerControls.Throw.performed += ThrowPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
        controls.PlayerControls.Aim.performed += AimPerformed;
        controls.PlayerControls.Aim.canceled += AimCanceled;
        controls.PlayerControls.Jump.performed += JumpPerformed;
        controls.PlayerControls.Interact.performed += InteractPerformed;

        inputSystemUIInputModule.submit = InputActionReference.Create(controls.UI.Submit);
        inputSystemUIInputModule.move = InputActionReference.Create(controls.UI.Navigate);

        //mouse and keyboard fight with each other, when menus are opened a tracker should activate which checks last device used to enable and disable.
        /*inputSystemUIInputModule.point = InputActionReference.Create(controls.UI.Point);
        inputSystemUIInputModule.leftClick = InputActionReference.Create(controls.UI.Click);*/

        EnablePlayerInputs();
        DisableUIInputs();
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        InteractEventPerformed?.Invoke();
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        JumpEventPerformed?.Invoke();
    }

    private void ThrowStarted(InputAction.CallbackContext context)
    {
        ThrowEventStarted?.Invoke();
    }

    private void ThrowPerformed(InputAction.CallbackContext context)
    {
        ThrowEventPerformed?.Invoke();
    }

    private void AimPerformed(InputAction.CallbackContext context)
    {
        AimEventPerformed?.Invoke(context.ReadValue<Vector2>());
    }

    private void AimCanceled(InputAction.CallbackContext context)
    {
        AimEventCancelled?.Invoke();
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {
        MoveEventPerfomed?.Invoke(context.ReadValue<Vector2>());
    }

    private void MoveCanceled(InputAction.CallbackContext context)
    {
        MoveEventCancelled?.Invoke();
    }

    public void EnablePlayerInputs()
    {
        controls.PlayerControls.Enable();
    }

    public void DisablePlayerInputs()
    {
        controls.PlayerControls.Disable();
    }

    public void EnableUIInputs()
    {
        controls.UI.Enable();
    }

    public void DisableUIInputs()
    {
        controls.UI.Disable();
    }

    public void EnableAllInputs()
    {
        controls.PlayerControls.Enable();
        controls.UI.Enable();
    }

    public void DisableAllInputs()
    {
        controls.PlayerControls.Disable();
        controls.UI.Disable();
    }

    private void OnEnable()
    {
        if(controls != null)
        {
            EnableAllInputs();
        }  
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    private void OnDestroy()
    {
        if (controls == null) return;

        controls.PlayerControls.Throw.started -= ThrowStarted;
        controls.PlayerControls.Throw.performed -= ThrowPerformed;
        controls.PlayerControls.Movement.performed -= MovePerformed;
        controls.PlayerControls.Movement.canceled -= MoveCanceled;
        controls.PlayerControls.Aim.performed -= AimPerformed;
        controls.PlayerControls.Aim.canceled -= AimCanceled;
        controls.PlayerControls.Jump.performed -= JumpPerformed;
        controls.PlayerControls.Interact.performed -= InteractPerformed;
    }
}

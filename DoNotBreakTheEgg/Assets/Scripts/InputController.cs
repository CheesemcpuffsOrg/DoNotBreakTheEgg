using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    Controls controls;

    public event Action ThrowEventStarted;
    public event Action ThrowEventPerformed;
    public event Action<Vector2> AimEventPerformed;
    public event Action AimEventCancelled;
    public event Action<Vector2> MoveEventPerfomed;
    public event Action MoveEventCancelled;
    public event Action JumpEventPerformed;

    private void Awake()
    {
        controls = new();

        controls.PlayerControls.Throw.started += ThrowStarted;
        controls.PlayerControls.Throw.performed += ThrowPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
        controls.PlayerControls.Aim.performed += AimPerformed;
        controls.PlayerControls.Aim.canceled += AimCanceled;
        controls.PlayerControls.Jump.performed += JumpPerformed;
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

    private void OnEnable()
    {
        controls.Enable();  
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

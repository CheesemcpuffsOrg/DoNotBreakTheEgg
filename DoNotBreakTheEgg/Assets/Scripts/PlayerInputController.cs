using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{

    Controls controls;

    [SerializeField] float moveSpeed = 5f;

    public event Action ThrowEventStarted;
    public event Action ThrowEventPerformed;
    public event Action<Vector2> AimEventPerformed;
    public event Action AimEventCancelled;

    private float input;
    
    private void Awake()
    {
        controls = new();

        controls.PlayerControls.Throw.started += ThrowStarted;
        controls.PlayerControls.Throw.performed += ThrowPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
        controls.PlayerControls.Aim.performed += AimPerformed;
        controls.PlayerControls.Aim.canceled += AimCanceled;
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = new Vector2(input * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(movement);
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
        input = context.ReadValue<Vector2>().x;
    }

    private void MoveCanceled(InputAction.CallbackContext context)
    {
        input = 0; // Reset input when movement is canceled
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

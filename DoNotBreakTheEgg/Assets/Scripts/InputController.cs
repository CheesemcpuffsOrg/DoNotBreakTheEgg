using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    Controls controls;

    [SerializeField] float moveSpeed = 5f;

    private event Action throwEventStarted;
    public event Action ThrowEventStarted
    {
        add { throwEventStarted += value; }
        remove { throwEventStarted -= value; }
    }

    private event Action throwEventPerformed;
    public event Action ThrowEventPerformed
    {
        add { throwEventPerformed += value; }
        remove { throwEventPerformed -= value; }
    }

    private float input;
    
    private void Awake()
    {

        controls = new();

        controls.PlayerControls.Throw.started += ThrowStarted;
        controls.PlayerControls.Throw.performed += ThrowPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
        //controls.PlayerControls.Interact.performed += InteractPerformed;
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
        throwEventStarted?.Invoke();
    }

    private void ThrowPerformed(InputAction.CallbackContext context)
    {
        throwEventPerformed?.Invoke();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimComponent : MonoBehaviour, IAimComponent
{
    enum ControllerType
    {
        Gamepad,
        Mouse
    }

    [SerializeField] Transform transformToRotate;
    [SerializeField] ControllerType controllerType;


    private Vector2 aimInput;
    private Quaternion targetRotation;

    private void Update()
    {
        // For gamepad input (normalized vector)
        if (controllerType == ControllerType.Gamepad)
        {
            // Apply the deadzone
            Vector2 processedInput = ApplyDeadzone(aimInput);

            if (processedInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(processedInput.y, processedInput.x) * Mathf.Rad2Deg;
                targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }
            else
            {
                // Stop adjusting rotation when input is zero
                targetRotation = transformToRotate.rotation;
            }

            // Smoothly interpolate to the target rotation
            transformToRotate.rotation = Quaternion.Slerp(transformToRotate.rotation, targetRotation, Time.deltaTime * 5f);
        }
        // For mouse input (world position)
        else if (controllerType == ControllerType.Mouse)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0f; // Ensure z-axis is zero for 2D

            Vector3 direction = mousePosition - transformToRotate.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transformToRotate.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    public void MoveAim(Vector2 direction)
    {
        aimInput = direction;
    }

    public void StopAim()
    {
        aimInput = Vector2.zero;
    }

    private Vector2 ApplyDeadzone(Vector2 input, float deadzone = 0.15f)
    {
        if (input.sqrMagnitude < deadzone * deadzone)
            return Vector2.zero; // Clamp small inputs to zero
        return input;
    }

    
}

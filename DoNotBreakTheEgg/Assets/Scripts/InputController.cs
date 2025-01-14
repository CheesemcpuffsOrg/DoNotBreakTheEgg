using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    Controls controls;

    [SerializeField] GameObject projectile;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float powerBase = 1f;
    [SerializeField] float powerMax = 10f;
    float powerCurrent;
    [SerializeField] float moveSpeed = 5f;

    bool chargingShot;

    private float input;
    
    private void Awake()
    {

        controls = new();

        controls.PlayerControls.Shoot.started += ShootStarted;
        controls.PlayerControls.Shoot.performed += ShootPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
    }

    private void Update()
    {
        Movement();
        ChargeShot();
    }

    private void ChargeShot()
    {
        if (!chargingShot)
        {
            return;
        }

        powerCurrent += 0.02f;

        Debug.Log(powerCurrent);

        // Cap the power at powerMax
        if (powerCurrent >= powerMax)
        {
            powerCurrent = powerMax;
        }
    }

    private void Movement()
    {
        Vector3 movement = new Vector2(input * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(movement);
    }

    private void ShootStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot started");
        powerCurrent = powerBase; // Reset power to the base value
        chargingShot = true; // Start charging
        
    }

    private void ShootPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot performed");
        var spawnedObj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        spawnedObj.GetComponent<Rigidbody2D>().velocity = powerCurrent * spawnPoint.up;
        chargingShot = false;
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

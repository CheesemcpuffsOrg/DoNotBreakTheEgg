using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    Controls controls;

    [SerializeField] GameObject projectile;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float speed = 10f;
    [SerializeField] float moveSpeed = 5f;

    private float input;
    
    private void Awake()
    {

        controls = new();

        controls.PlayerControls.Shoot.performed += ShootPerformed;
        controls.PlayerControls.Movement.performed += MovePerformed;
        controls.PlayerControls.Movement.canceled += MoveCanceled;
    }

    private void Update()
    {
        Vector3 movement = new Vector2(input * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(movement);
    }

    private void ShootPerformed(InputAction.CallbackContext context)
    {
        var spawnedObj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        spawnedObj.GetComponent<Rigidbody2D>().velocity = speed * spawnPoint.up;
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

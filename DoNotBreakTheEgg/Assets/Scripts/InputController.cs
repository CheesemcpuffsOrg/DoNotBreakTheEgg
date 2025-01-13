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
    
    private void Awake()
    {

        controls = new();

        controls.PlayerControls.Shoot.performed += Shoot_performed;
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        var spawnedObj = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        spawnedObj.GetComponent<Rigidbody2D>().velocity = speed * spawnPoint.up;
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

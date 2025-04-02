using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowEntity : MonoBehaviour
{
    [SerializeField] MonoEntity entityToFollow;
    [SerializeField] float smoothSpeed = 5f; // Adjust for smooth movement
    [SerializeField] float minY; // The lowest point the camera can go

    private float screenCenterY;
    private IAnchoringComponent anchoringComponent;

    private void Start()
    {
        screenCenterY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).y;

        anchoringComponent = entityToFollow.GetEntityComponent<IAnchoringComponent>();
    }

    private void FixedUpdate()
    {
        if(transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        if (!HoldEntityManager.Instance.IsEntityHeld(entityToFollow)) return;

        float targetY = transform.position.y;

        // Move up if player exceeds the center
        if (anchoringComponent.GetPosition().y > screenCenterY)
        {
            targetY = anchoringComponent.GetPosition().y;
        }
        // Move down if player falls below center (but not below minY)
        else if (anchoringComponent.GetPosition().y < screenCenterY && transform.position.y > minY)
        {
            targetY = anchoringComponent.GetPosition().y;
        }

        // Smoothly move the camera to the new Y position
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), smoothSpeed * Time.deltaTime);
    }
}

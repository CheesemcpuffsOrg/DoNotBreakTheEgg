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

        transform.position = new Vector3(transform.position.x, minY, transform.position.z);
    }

    private void LateUpdate()
    {
        if (!HoldEntityManager.Instance.IsEntityHeld(entityToFollow)) return;

        float screenCenterY = transform.position.y; // Dynamically recalculate

        float entityY = anchoringComponent.GetPosition().y;
        float targetY = transform.position.y;

        if (entityY > screenCenterY)
        {
            targetY = entityY;
        }
        else if (entityY < screenCenterY && transform.position.y > minY)
        {
            targetY = Mathf.Max(entityY, minY); // Clamp to minY
        }

        // Smoothly move the camera to the new Y position
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), smoothSpeed * Time.deltaTime);
    }
}

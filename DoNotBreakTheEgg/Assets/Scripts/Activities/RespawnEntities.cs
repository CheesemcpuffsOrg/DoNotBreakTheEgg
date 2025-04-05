using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespawnEntities : MonoBehaviour
{

    [SerializeField] GameObject transporterPadPrefab;
    [SerializeField] LayerMask spawnLayerMask;

    public void RespawnEntity(IEntity entity)
    {
        EntityRegistry.RegisterEntity(entity);

        var spawnLocation = CameraUtility.GetRandomLocationOutsideViewport(Camera.main, 1, ViewportSide.XAxis);

        var landingZone = FindLandingZone();

        //tween to the location
    }

    Vector2 FindLandingZone()
    {
        // Get screen bounds in world space
        Vector2 screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector2 screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);

        int verticalRayCount = 20;
        float offset = 0.05f;

        // Spacing between each ray along the top of the screen
        float verticalRaySpacing = (screenMax.x - screenMin.x) / (verticalRayCount - 1);

        for (int i = 0; i < verticalRayCount; i++)
        {
            // Calculate x position for each ray along the top of the screen
            float xPos = screenMin.x + (i * verticalRaySpacing);
            Vector2 rayOrigin = new Vector2(xPos, screenMax.y - offset); // Position at the top of the screen

            // Shoot a ray downward
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, spawnLayerMask);

            Debug.DrawRay(rayOrigin, Vector2.down * 5f, Color.red, 2f); // Visualize in Scene view

            if (hit.collider != null)
            {
                return hit.point;
            }
        }

        return screenMin;
    }
}

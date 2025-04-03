using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtility
{
    public static bool IsInsideViewport(Camera camera, Vector3 position, float offset = 0)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(position);

        return viewportPoint.x >= 0 - offset && viewportPoint.x <= 1 + offset &&
           viewportPoint.y >= 0 - offset && viewportPoint.y <= 1 + offset;
    }
}

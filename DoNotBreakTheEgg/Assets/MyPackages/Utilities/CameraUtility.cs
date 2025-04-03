using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtility
{
    public static bool IsTransformOutsideView(Camera camera, Transform transform)
    {
        if (camera == null || transform == null) return true;

        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);

        return viewportPoint.x < 0 || viewportPoint.x > 1 ||
               viewportPoint.y < 0 || viewportPoint.y > 1 ||
               viewportPoint.z < 0;
    }
}

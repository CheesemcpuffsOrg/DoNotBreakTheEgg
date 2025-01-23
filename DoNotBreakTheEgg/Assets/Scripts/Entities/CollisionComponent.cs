using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour, ICollisionComponent
{

    [SerializeField] List<Collider2D> allPlayerColliders;

    public List<Collider2D> AllPlayerColliders => allPlayerColliders;

    IEntity entity;

    [SerializeField] BoxCollider2D playerCollider;

    [SerializeField] int horizontalRayCount = 4;
    public int HorizontalRayCount => horizontalRayCount;
    [SerializeField] int verticalRayCount = 4;
    public int VerticalRayCount => verticalRayCount;

    const float skinWidth = 0.015f;
    public float SkinWidth => skinWidth;

    float horizontalRaySpacing;
    public float HorizontalRaySpacing => horizontalRaySpacing;

    float verticalRaySpacing;
    public float VerticalRaySpacing => verticalRaySpacing;

    RaycastOriginsStruct raycastOrigins;
    public RaycastOriginsStruct RaycastOrigins => raycastOrigins;
   

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<IEntity>();

        CalculateRaySpacing();

        foreach (var collider in allPlayerColliders)
        {
            EntityCollisionService.RegisterEntityCollider(collider, entity);
        }
    }

    public void UpdateRaycastOrigins()
    {
        var bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topleft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topright = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        var bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}

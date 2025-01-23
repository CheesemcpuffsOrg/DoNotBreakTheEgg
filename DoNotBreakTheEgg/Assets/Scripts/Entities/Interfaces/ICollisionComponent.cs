using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionComponent : IEntityComponent
{
    public List<Collider2D> AllPlayerColliders { get;}

    public int HorizontalRayCount { get;}
    public float HorizontalRaySpacing { get; }

    public int VerticalRayCount { get; }
    public float VerticalRaySpacing { get; }

    public float SkinWidth { get; }

    public RaycastOriginsStruct RaycastOrigins { get; }

    public void UpdateRaycastOrigins();
}

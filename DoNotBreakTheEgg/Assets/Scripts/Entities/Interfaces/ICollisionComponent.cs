using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionComponent : IEntityComponent
{
    
    public Bounds GetMainPlayerColliderBounds();

    public bool IsEntityCollider(Collider2D collider);
}

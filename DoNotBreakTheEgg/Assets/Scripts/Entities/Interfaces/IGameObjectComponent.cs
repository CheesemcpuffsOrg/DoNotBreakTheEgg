using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectComponent : IEntityComponent
{
    public Transform GetTransform();

    public Vector3 GetPosition();

    public void SetPosition(Vector3 position);

    public void SetParent(Transform parentTransform);

    public void Destroy();
}

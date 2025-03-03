using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnchoringComponent : IEntityComponent
{
    public Vector3 GetPosition();

    public void SetPosition(Vector3 position);

    public void SetParent(Transform parentTransform);

    public Transform GetAnchor(AnchorScriptableObject anchor);
}

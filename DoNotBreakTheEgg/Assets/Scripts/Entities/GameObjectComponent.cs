using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameObjectComponent : MonoBehaviour, IGameObjectComponent
{
    [Serializable]
    private class AnchorMapping
    {
        public AnchorScriptableObject anchorScriptableObject;
        public Transform anchor;
    }

    [SerializeField] List<AnchorMapping> anchorMappings;

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetPosition() 
    { 
        return transform.position; 
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position; 
    }

    public void SetParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

    public void Destroy(float time)
    {

        gameObject.GetComponent<ITagComponent>().RemoveAllTags();

        Destroy(gameObject, time);
    }

    public Transform GetAnchor(AnchorScriptableObject anchor)
    {
        foreach (AnchorMapping mapping in anchorMappings)
        {
            if(mapping.anchorScriptableObject == anchor)
            {
                return mapping.anchor;
            }
        }

        return null;
    }
}

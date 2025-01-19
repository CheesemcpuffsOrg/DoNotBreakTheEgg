
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagComponent : MonoBehaviour, ITagComponent
{
    [SerializeField] private List<TagScriptableObject> tags;

    private void Awake()
    {
        foreach (var tag in tags)
            gameObject.RegisterGameObjectWithTag(tag);
    }

    public List<TagScriptableObject> GetTags()
    {
        return tags;
    }

    public void SetTags(params TagScriptableObject[] tags)
    {
        this.tags = tags.ToList();
    }
}

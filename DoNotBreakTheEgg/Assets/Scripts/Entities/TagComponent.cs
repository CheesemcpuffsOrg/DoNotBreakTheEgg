
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

    public void AddTag(TagScriptableObject tag)
    {
        gameObject.RegisterGameObjectWithTag(tag);
    }

    public void AddTags(TagScriptableObject[] tagsToAdd)
    {
        foreach (var tag in tagsToAdd)
            gameObject.RegisterGameObjectWithTag(tag);
    }

    public void RemoveTag(TagScriptableObject tag)
    {
        gameObject.UnregisterGameObjectWithTag(tag);
    }

    public void RemoveAllTags()
    {
        var reversedList = tags;

        reversedList.Reverse();

        foreach (var tag in reversedList)
            gameObject.UnregisterGameObjectWithTag(tag);
    }
}

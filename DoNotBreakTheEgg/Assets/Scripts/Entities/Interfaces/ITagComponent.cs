using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITagComponent : ITags, IEntityComponent
{
    public void AddTag(TagScriptableObject tag);

    public void AddTags(TagScriptableObject[] tagsToAdd);

    public void RemoveTag(TagScriptableObject tag);

    public void RemoveAllTags();
}

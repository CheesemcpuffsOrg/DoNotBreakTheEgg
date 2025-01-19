using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITagComponent : ITags, IEntityComponent
{
    public bool HasTag(TagScriptableObject tag);

    public bool HasAllTags(TagScriptableObject[] tags);
}

using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITagComponent : ITags, IEntityComponent
{
    public Observable<TagScriptableObject> TagAddedStream { get; }

    public Observable<TagScriptableObject> TagRemovedStream { get; }

    public bool HasTag(TagScriptableObject tag); 

    
}

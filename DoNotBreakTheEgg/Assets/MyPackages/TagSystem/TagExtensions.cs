using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TagExtensions
{
    // Automatically filled on Awake by Tags component.
    private static readonly Dictionary<TagScriptableObject, HashSet<GameObject>> allObjectsWithTag = new Dictionary<TagScriptableObject, HashSet<GameObject>>();
    // Tags component is cached per-instance when we call one of the HasTag methods.
    private static readonly ConditionalWeakTable<GameObject, ITags> cachedTags = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        // Tags.Awake() is not getting called on next Fast Play Mode, so we need to keep it
        //allObjectsWithTag.Clear();

        // Not sure about that one, maybe clear it only on scene change as well?
        cachedTags.Clear();
    }

    #region FindGameObjectWithTag
    public static GameObject FindWithTag(this GameObject gameObject, TagScriptableObject tag)
    {
        CheckFindWithTagArguments(tag);

        if (!allObjectsWithTag.TryGetValue(tag, out var objectsLookup) || objectsLookup.Count == 0)
        {
            throw new NullReferenceException($"No Game Objects found with tag {tag.name}.");
        }

        GameObject firstObject = null;
        foreach (var obj in objectsLookup)
        {
            firstObject = obj;
            break;
        }

        return firstObject;
    }
    public static HashSet<GameObject> FindAllWithTag(this GameObject gameObject, TagScriptableObject tag)
    {
        CheckFindWithTagArguments(tag);

        if (!allObjectsWithTag.TryGetValue(tag, out var objectsLookup) || objectsLookup.Count == 0)
        {
            throw new NullReferenceException($"No Game Objects found with tag {tag.name}.");
        }

        return objectsLookup;
    }

    public static bool TryFindWithTag(this GameObject gameObject, TagScriptableObject tag, out GameObject objectWithTag)
    {
        CheckFindWithTagArguments(tag);

        objectWithTag = null;
        if (!allObjectsWithTag.TryGetValue(tag, out var objectsLookup) || objectsLookup.Count == 0)
        {
            return false;
        }

        foreach (var obj in objectsLookup)
        {
            objectWithTag = obj;
            break;
        }
        return true;
    }
    public static bool TryFindAllWithTag(this GameObject gameObject, TagScriptableObject tag, out HashSet<GameObject> objectsLookup)
    {
        CheckFindWithTagArguments(tag);
        return allObjectsWithTag.TryGetValue(tag, out objectsLookup) && objectsLookup.Count != 0;
    }

    private static void CheckFindWithTagArguments(TagScriptableObject tag)
    {
        if (!tag)
            throw new NullReferenceException("Trying to find with none tag.");
    }
    #endregion

    #region HasTag
    public static bool HasTag(this Component instance, TagScriptableObject tag)
    {
        return HasTag(instance.gameObject, tag);
    }
    public static bool HasTag(this GameObject gameObject, TagScriptableObject tag)
    {
        if (!TryGetTagComponent(gameObject, out var component))
            return false;

        var compareTags = component.GetTags();
        if (compareTags.Contains(tag))
        {
            Debug.Log($"Tag {tag.name} found on {gameObject.name}.");
            return true;
        }

        Debug.Log($"Tag {tag.name} NOT found on {gameObject.name}.");
        return false;
    }

    public static bool HasOnlyTag(this Component instance, TagScriptableObject tag)
    {
        return HasOnlyTag(instance.gameObject, tag);
    }
    public static bool HasOnlyTag(this GameObject gameObject, TagScriptableObject tag)
    {
        if (!TryGetTagComponent(gameObject, out var component))
            return false;

        var compareTags = component.GetTags();

        if (compareTags.Count > 1)
            return false;

        if (compareTags.Count == 0 || compareTags[0] != tag)
            return false;

        return true;
    }

    public static bool HasAnyTag(this Component instance, params TagScriptableObject[] tags)
    {
        return HasAnyTag(instance.gameObject, tags);
    }
    public static bool HasAnyTag(this GameObject gameObject, params TagScriptableObject[] tags)
    {
        var compareTags = gameObject.GetComponent<ITags>()?.GetTags();
        if (compareTags == null || compareTags.Count == 0)
        {
            return false;
        }

        foreach (var tag in tags)
        {
            if (compareTags.Contains(tag))
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasAllTags(this Component instance, params TagScriptableObject[] tags)
    {
        return HasAllTags(instance.gameObject, tags);
    }
    public static bool HasAllTags(this GameObject gameObject, params TagScriptableObject[] tags)
    {
        if (!TryGetTagComponent(gameObject, out var component))
            return false;

        var compareTags = component.GetTags();
        foreach (var tag in tags)
        {
            if (!compareTags.Contains(tag))
                return false; // Immediately return false if any tag is missing
        }

        return true; // All tags are present
    }
    #endregion

    /// <summary>
    /// Not recommended practice, as it depends on GameObject hierarchy. Use only for fast prototyping.<para></para>
    /// Returns null when there is no root child with given tag.
    /// </summary>
    public static bool TryGetRootChildWithTag(this Transform transform, TagScriptableObject tag, out Transform child)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            child = transform.GetChild(i);
            if (TryGetTagComponent(child.gameObject, out var component))
                return true;
        }

        child = null;
        return false;
    }


    internal static void RegisterGameObjectWithTag(this GameObject gameObject, TagScriptableObject tag)
    {
        if (!allObjectsWithTag.TryGetValue(tag, out var objectsList))
        {
            objectsList = new HashSet<GameObject>();
            allObjectsWithTag.Add(tag, objectsList);
        }

        if (!objectsList.Contains(gameObject))
        {
            objectsList.Add(gameObject);
            //Debug.Log($"Registered '{gameObject.name}' with global tag '{tag.name}'.");
        }
    }

    internal static void UnregisterGameObjectWithTag(this GameObject gameObject, TagScriptableObject tag)
    {
        if (allObjectsWithTag.TryGetValue(tag, out var objectsList))
        {
            if (objectsList.Contains(gameObject))
            {
                objectsList.Remove(gameObject);
                // Debug.Log($"{tag} has been removed from {gameObject}");
            }
        }
    }

    private static bool TryGetTagComponent(GameObject gameObject, out ITags component)
    {
        if (!cachedTags.TryGetValue(gameObject, out component))
        {
            if (!gameObject.TryGetComponent(out component))
                return false;

            cachedTags.Add(gameObject, component);
        }
        return true;
    }
}

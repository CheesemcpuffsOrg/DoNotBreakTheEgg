using System.Collections.Generic;
using UnityEngine;

public static class AudioSystem
{
    public enum AudioObjType 
    { 
        STATIC, 
        FOLLOW 
    }

    static Queue<GameObject> staticAudioPool = new Queue<GameObject>();
    static Queue<GameObject> followAudioPool = new Queue<GameObject>();

    /// <summary>
    /// Generate a gameobject and AudioSource to use.
    /// </summary>
    public  static (GameObject, AudioSource) GenerateAudioSource(AudioScriptableObject sound, AudioVariant variant, Vector3 location, AudioObjType type, Transform transformLocation = null)
    {
        if (sound == null)
        {
            Debug.LogError($"You are missing a sound scriptable object");
            return (null, null);
        }

        GameObject obj = AudioObjectType(location, type, transformLocation);

        var audioSource = obj.GetComponent<AudioSource>();

        PopulateTheAudioSource(sound, variant, audioSource);

        return (obj, audioSource);
    }

    /// <summary>
    /// Clear an Audio Source object and stop the sound.
    /// </summary>
    public static void ClearAudioSource(AudioObjType type, GameObject audioSourceObject)
    {
        
        if (type == AudioObjType.STATIC)
        {
            staticAudioPool.Enqueue(audioSourceObject);
        }
        else
        {
            audioSourceObject.GetComponent<AudioFollowTransform>().RemoveTransform();
            followAudioPool.Enqueue(audioSourceObject);
        }

        var audioSource = audioSourceObject.GetComponent<AudioSource>();

        audioSource.Stop();
    }

    /// <summary>
    /// Creates or re-uses an audio object.
    /// </summary>
    private static GameObject AudioObjectType(Vector3 location, AudioObjType type, Transform transformLocation)
    {
        GameObject obj;

        if (type == AudioObjType.STATIC)
        {
            //take obj from audio pool, if there are none create a new object.
            if (staticAudioPool.Count > 0)
            {
                obj = staticAudioPool.Dequeue();
            }
            else
            {
                obj = new GameObject("StaticAudioObject");
                obj.AddComponent<AudioSource>();
            }

            obj.transform.position = location;
        }
        else
        {
            //take obj from audio pool, if there are none create a new object.
            if (followAudioPool.Count > 0)
            {
                obj = followAudioPool.Dequeue();

            }
            else
            {
                obj = new GameObject("FollowAudioObject");
                obj.AddComponent<AudioSource>();
                obj.AddComponent<AudioFollowTransform>();
            }

            obj.transform.position = location;
            obj.GetComponent<AudioFollowTransform>().AssignTransform(transformLocation);
        }

        return obj;
    }

    private static void PopulateTheAudioSource(AudioScriptableObject sound, AudioVariant audioVariant, AudioSource audioSource)
    {
        audioSource.clip = audioVariant.audioClip;

        audioSource.playOnAwake = false;

        audioSource.pitch = audioVariant.pitch;
        audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        audioSource.loop = sound.loop;
        audioSource.panStereo = sound.pan;
        audioSource.spatialBlend = sound.spatialBlend;
        audioSource.dopplerLevel = sound.dopplerLevel;
        audioSource.minDistance = sound.minDistance;
        audioSource.maxDistance = sound.maxDistance;
        audioSource.rolloffMode = sound.volumeRollOffMode;

        if (sound.volumeRollOffMode == AudioRolloffMode.Custom)
        {
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, sound.volumeRollOffCurve);
        }
    }
}

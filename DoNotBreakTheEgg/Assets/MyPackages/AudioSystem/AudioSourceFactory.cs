using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioSourceFactory
{
    public class AudioSourceData
    {
        public bool PlayOnAwake { get; }

        public AudioMixerGroup MixerGroup { get; }
        public bool Loop { get; }
        public float Pan { get; }
        public float SpatialBlend { get; }
        public float DopplerLevel { get; }
        public float MinDistance { get; }
        public float MaxDistance { get; }
        public AudioRolloffMode RolloffMode { get; }
        public AnimationCurve Curve { get; }

        public AudioSourceData(AudioMixerGroup audioMixerGroup, bool loop, float pan, float spatialBlend, float dopplerLevel, float minDistance, float maxDistance, AudioRolloffMode audioRolloffMode, AnimationCurve curve)
        {
            PlayOnAwake = false;

            MixerGroup = audioMixerGroup;
            Loop = loop;
            Pan = pan;
            SpatialBlend = spatialBlend;
            DopplerLevel = dopplerLevel;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            RolloffMode = audioRolloffMode;
            Curve = curve;
        }
    }

    public enum AudioObjType 
    { 
        STATIC, 
        FOLLOW 
    }

    static Dictionary<AudioObjType, Queue<GameObject>> audioPoolDictionary = new Dictionary<AudioObjType, Queue<GameObject>>()
    {
        { AudioObjType.STATIC, new Queue<GameObject>() },
        { AudioObjType.FOLLOW, new Queue<GameObject>() }
    };

    /// <summary>
    /// Generate a gameobject and AudioSource to use.
    /// </summary>
    public  static (GameObject, AudioSource) GenerateAudioSource(AudioSourceData data, AudioVariant variant, Vector3 location, AudioObjType type, Transform transformLocation = null)
    {
        if (data == null)
        {
            Debug.LogError($"You are missing a sound scriptable object");
            return (null, null);
        }

        var obj = GetOrCreateAudioObject(location, type, transformLocation);

        var audioSource = obj.GetComponent<AudioSource>();

        PopulateTheAudioSource(data, variant, audioSource);

        return (obj, audioSource);
    }

    /// <summary>
    /// Clear an Audio Source object and stop the sound.
    /// </summary>
    public static void ClearAudioSource(AudioObjType type, GameObject audioSourceObject)
    {

        var audioSource = audioSourceObject.GetComponent<AudioSource>();
        audioSource.Stop();

        switch (type)
        {
            case AudioObjType.STATIC:
                audioPoolDictionary[type].Enqueue(audioSourceObject);
                break;
            case AudioObjType.FOLLOW:
                audioSourceObject.GetComponent<AudioFollowTransform>().RemoveTransform();
                audioPoolDictionary[type].Enqueue(audioSourceObject);
                break;
            default:
                Debug.LogWarning($"Unhandled AudioObjType: {type}");
                break;
        }
    }

    /// <summary>
    /// Creates or re-uses an audio object.
    /// </summary>
    private static GameObject GetOrCreateAudioObject(Vector3 location, AudioObjType type, Transform transformLocation)
    {

        GameObject obj = null;

        if (audioPoolDictionary.TryGetValue(type, out var queue))
        {
            if(queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
        }

        switch (type)
        {
            case AudioObjType.STATIC:
                if (obj != null)
                {
                    obj = new GameObject("StaticAudioObject");
                    obj.AddComponent<AudioSource>();
                }
                break;
            case AudioObjType.FOLLOW:
                if (obj != null)
                {
                    obj = new GameObject("FollowAudioObject");
                    obj.AddComponent<AudioSource>();
                    obj.AddComponent<AudioFollowTransform>();
                }
                obj.GetComponent<AudioFollowTransform>().AssignTransform(transformLocation);
                break;
            default:
                Debug.LogWarning($"Unhandled AudioObjType: {type}");
                break;
        }

        obj.transform.position = location;

        return obj;
    }

    private static void PopulateTheAudioSource(AudioSourceData data, AudioVariant audioVariant, AudioSource audioSource)
    {
        audioSource.clip = audioVariant.audioClip;
        audioSource.volume = audioVariant.volume;

        audioSource.playOnAwake = data.PlayOnAwake;

        audioSource.pitch = audioVariant.pitch;
        audioSource.outputAudioMixerGroup = data.MixerGroup;
        audioSource.loop = data.Loop;
        audioSource.panStereo = data.Pan;
        audioSource.spatialBlend = data.SpatialBlend;
        audioSource.dopplerLevel = data.DopplerLevel;
        audioSource.minDistance = data.MinDistance;
        audioSource.maxDistance = data.MaxDistance;
        audioSource.rolloffMode = data.RolloffMode;

        if (data.RolloffMode == AudioRolloffMode.Custom && data.Curve != null)
        {
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, data.Curve);
        }
    }
}

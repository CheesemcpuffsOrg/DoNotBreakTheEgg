using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioSystem
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

    static Queue<GameObject> staticAudioPool = new Queue<GameObject>();
    static Queue<GameObject> followAudioPool = new Queue<GameObject>();

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

        GameObject obj = AudioObjectType(location, type, transformLocation);

        var audioSource = obj.GetComponent<AudioSource>();

        PopulateTheAudioSource(data, variant, audioSource);

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

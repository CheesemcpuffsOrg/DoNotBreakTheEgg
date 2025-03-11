using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static AudioSystem;

public class AudioSystemManager : MonoBehaviour
{
    private class AudioReference
    {
        public AudioScriptableObject ScriptableObjectReference { get; }
        public UniqueSoundID UUID { get; }
        public GameObject AudioSourceObject { get; }
        public AudioObjType Type { get; }
        public Coroutine ClipLength { get; }
        public UnityEvent EndOfClip { get; }
        public float Volume { get; }

        public AudioReference(AudioScriptableObject scriptableObjectReference, UniqueSoundID UUID, GameObject audioSourceObject, AudioObjType type, Coroutine clipLength, UnityEvent endOfClip, float volume)
        {
            ScriptableObjectReference = scriptableObjectReference;
            this.UUID = UUID;
            AudioSourceObject = audioSourceObject;
            Type = type;
            ClipLength = clipLength;
            EndOfClip = endOfClip;
            Volume = volume;
        }
    }

    /// <summary>
    /// Base functionality required when calling a sound
    /// </summary>
    private bool PlaySound(AudioScriptableObject sound, UniqueSoundID UUID, Vector3 location, AudioObjType type, string fullStackTrace, Transform transformLocation = null)
    {
        if (sound == null)
        {
            Debug.LogError($"You are missing a sound scriptable object");
            return false;
        }

        var chosenAudioVariant = RandomUtility.ObjectPoolCalculator(sound.audioClips);

        var (obj, audioSource) = GenerateAudioSource(sound, chosenAudioVariant, location, type, transformLocation);

        //CreateAudioReference(sound, UUID, obj, audioSource, chosenAudioVariant, type);

        audioSource.Play();

        return true;
    }
}

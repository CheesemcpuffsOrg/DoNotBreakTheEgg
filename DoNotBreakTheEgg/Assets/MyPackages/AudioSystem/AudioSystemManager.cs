using System;
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
        public string SoundID { get; }
        public GameObject AudioSourceObject { get; }
        public AudioObjType Type { get; }
        public Coroutine ClipLength { get; }
        public UnityEvent EndOfClip { get; }
        public float Volume { get; }

        public AudioReference(AudioScriptableObject scriptableObjectReference, UniqueSoundID UUID, GameObject audioSourceObject, AudioObjType type, Coroutine clipLength, UnityEvent endOfClip, float volume)
        {
            ScriptableObjectReference = scriptableObjectReference;
            SoundID = UUID.soundID;
            AudioSourceObject = audioSourceObject;
            Type = type;
            ClipLength = clipLength;
            EndOfClip = endOfClip;
            Volume = volume;
        }
    }

    List<AudioReference> audioReferences = new List<AudioReference>();
    Transform audioPoolContainer;

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

        var audioData = new AudioSourceData(sound.audioMixerGroup, sound.loop, sound.pan, sound.spatialBlend, sound.dopplerLevel, sound.minDistance, sound.maxDistance, sound.volumeRollOffMode, sound.volumeRollOffCurve);

        var (obj, audioSource) = GenerateAudioSource(audioData, chosenAudioVariant, location, type, transformLocation);

        CreateAudioReference(sound, UUID, obj, audioSource, chosenAudioVariant, type);

        audioSource.Play();

        return true;
    }

    private void CreateAudioReference(AudioScriptableObject sound, UniqueSoundID UUID, GameObject obj, AudioSource audioSource, AudioVariant audioVariant, AudioObjType type)
    {

        Coroutine clipLength = null;

        if (!audioSource.loop)
        {
            var lengthCalculatingPitch = audioSource.clip.length / Math.Abs(audioVariant.pitch);
            clipLength = StartCoroutine(Countdown(lengthCalculatingPitch, UUID.soundID));


            var createdObjReference = new AudioReference(sound, UUID, obj, type, clipLength, new UnityEvent(), audioVariant.volume);

            audioReferences.Add(createdObjReference);
        }
    }

    /// <summary>
    /// This will turn any non-looping audio into a oneshot
    /// </summary>
    IEnumerator Countdown(float seconds, string soundID)
    {
        yield return new WaitForSeconds(seconds);

        foreach (var audioReference in audioReferences)
        {
            if (audioReference.SoundID == soundID)
            {
                audioReference.AudioSourceObject.transform.SetParent(audioPoolContainer);

                if (audioReference.EndOfClip != null)
                {
                    audioReference.EndOfClip.Invoke();
                }

                ClearAudioSource(audioReference.Type, audioReference.AudioSourceObject);

                audioReferences.Remove(audioReference);

                yield break;
            }
        }
    }
}

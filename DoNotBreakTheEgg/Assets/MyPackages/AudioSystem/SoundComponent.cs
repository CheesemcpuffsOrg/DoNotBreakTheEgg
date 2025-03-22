using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundComponent : MonoBehaviour, ISoundComponent
{

    UniqueSoundID UUID = new UniqueSoundID();

    public void PlaySound(SoundData data)
    {
        StartCoroutine(DelayTimer(data));
    }

    private IEnumerator DelayTimer(SoundData data)
    {
        yield return new WaitForSeconds(data.Delay);

        var playLocation = data.PlayLocation;

        if(playLocation == null)
        {
            playLocation = transform;
        }

        AdvancedAudioSystemManager.Instance.PlaySound(data.AudioScriptableObject, UUID, playLocation, data.FollowTransform, data.FadeInFadeOut);
    }

    public bool IsSoundPlaying(SoundData data)
    {
        return AdvancedAudioSystemManager.Instance.IsSoundPlaying(data.AudioScriptableObject, UUID);
    }

    public void StopSound(SoundData data)
    {
        AdvancedAudioSystemManager.Instance.StopSound(data.AudioScriptableObject, UUID, data.FadeInFadeOut);
    }

    //deprecated

    public void PlaySound(AudioScriptableObject audioScriptableObject, Vector3 location, UnityAction fireEventWhenSoundFinished = null)
    {
        OldAudioManager.AudioManagerInstance.PlaySound(audioScriptableObject, UUID, location, fireEventWhenSoundFinished);
    }

    public void PlaySound(AudioScriptableObject audioScriptableObject, Transform transformLocation, bool followTransform = false, UnityAction fireEventWhenSoundFinished = null)
    {
        OldAudioManager.AudioManagerInstance.PlaySound(audioScriptableObject, UUID, transformLocation, followTransform, fireEventWhenSoundFinished);
    }

    public void StopSound(AudioScriptableObject audioScriptableObject)
    {
        OldAudioManager.AudioManagerInstance.StopSound(audioScriptableObject, UUID);
    }

    public bool IsSoundPlaying(AudioScriptableObject audioScriptableObject)
    {
        return OldAudioManager.AudioManagerInstance.IsSoundPlaying(audioScriptableObject, UUID);
    }

    public void DynamicVolumePrioritySystem(AudioScriptableObject audioScriptableObject, bool systemIsActive)
    {
        OldAudioManager.AudioManagerInstance.DynamicVolumePrioritySystem(audioScriptableObject, systemIsActive);
    }
}

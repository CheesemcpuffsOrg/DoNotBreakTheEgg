using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISoundComponent
{
    public void PlaySound(SoundData data);
    public bool IsSoundPlaying(SoundData data);
    public void StopSound(SoundData data);

    //deprecated
    public void PlaySound(AudioScriptableObject audioScriptableObject, Vector3 location, UnityAction fireEventWhenSoundFinished = null);
    public void PlaySound(AudioScriptableObject audioScriptableObject, Transform transformLocation, bool followTransform = false, UnityAction fireEventWhenSoundFinished = null);
    public void StopSound(AudioScriptableObject audioScriptableObject);
    public bool IsSoundPlaying(AudioScriptableObject audioScriptableObject);
    public void DynamicVolumePrioritySystem(AudioScriptableObject audioScriptableObject, bool systemIsActive);
}

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

        AdvancedAudioSystemManager.Instance.PlaySound(data.AudioScriptableObject, UUID, playLocation, data.FollowTransform);
    }

    public bool IsSoundPlaying(SoundData data)
    {
        return AdvancedAudioSystemManager.Instance.IsSoundPlaying(data.AudioScriptableObject, UUID);
    }

    public void StopSound(SoundData data)
    {
        AdvancedAudioSystemManager.Instance.StopSound(data.AudioScriptableObject, UUID);
    }
}

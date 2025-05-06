using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmissionStrategy : MonoBehaviour, IEmissionStrategy, ISoundComponent
{

    UniqueSoundID UUID = new UniqueSoundID();

    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData soundEmission;

    public void Emit()
    {
        PlaySound(soundEmission);
    }

    public void Emit(IEntity entity)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySound(SoundData data)
    {
        if (data.AudioScriptableObject == null) return; //fails quitely if empty

        StartCoroutine(DelayTimer(data));
    }

    private IEnumerator DelayTimer(SoundData data)
    {
        yield return new WaitForSeconds(data.Delay);

        var playLocation = data.PlayLocation;

        if (playLocation == null)
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
        if (data.AudioScriptableObject == null) return; //fails quitely if empty

        AdvancedAudioSystemManager.Instance.StopSound(data.AudioScriptableObject, UUID);
    }
}

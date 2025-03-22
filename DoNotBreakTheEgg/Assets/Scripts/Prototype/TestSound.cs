using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData sound;
    [SerializeField] GameObject soundComponentObj;

    ISoundComponent soundComponent => soundComponentObj.GetComponent<ISoundComponent>();

    //Start is called before the first frame update
    void Start()
    {
        soundComponent.PlaySound(sound);

        StartCoroutine(KillSound());
    }

    IEnumerator KillSound()
    {
        yield return new WaitForSeconds(5);

        soundComponent.StopSound(sound);
    }
}

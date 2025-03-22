using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData sound;
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData sound2;
    [SerializeField] GameObject soundComponentObj;

    ISoundComponent soundComponent => soundComponentObj.GetComponent<ISoundComponent>();

    //Start is called before the first frame update
    void Start()
    {
        soundComponent.PlaySound(sound);
    }
}

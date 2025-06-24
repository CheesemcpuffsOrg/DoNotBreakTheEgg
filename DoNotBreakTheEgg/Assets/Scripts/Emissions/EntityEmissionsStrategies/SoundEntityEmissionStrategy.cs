using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEntityEmissionStrategy : MonoBehaviour, IEntityEmissionStrategy
{
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData soundEmission;

    public void Emit(IEntity entity)
    {
        entity.GetEntityComponent<IEntitySoundComponent>().PlaySound(soundEmission);
    }
}

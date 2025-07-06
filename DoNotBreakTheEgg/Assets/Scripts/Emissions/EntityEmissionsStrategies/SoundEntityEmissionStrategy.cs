using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEntityEmissionStrategy : MonoBehaviour, IEntityEmissionStrategy
{
    [Serializable]
    private class TagSoundDataMapping
    {
        [SerializeField] SoundData soundEmission;
        public SoundData SoundEmission => soundEmission;
        [SerializeField] TagScriptableObject tagScriptableObject;
        public TagScriptableObject TagScriptableObject => tagScriptableObject;
    }

    [SerializeField] List<TagSoundDataMapping> mappings = new List<TagSoundDataMapping>();

    [Header("Default Fall Back")]
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData defaultSoundEmission;

    public void Emit(IEntity entity)
    {
        if (!mappings.Any(mapping => entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject)) && defaultSoundEmission.AudioScriptableObject != null)
        {
            entity.GetEntityComponent<IEntitySoundComponent>().PlaySound(defaultSoundEmission);
        }

        foreach (var mapping in mappings)
        {
            if (entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject))
            {
                entity.GetEntityComponent<IEntitySoundComponent>().PlaySound(mapping.SoundEmission);
            }
        }
    }
}

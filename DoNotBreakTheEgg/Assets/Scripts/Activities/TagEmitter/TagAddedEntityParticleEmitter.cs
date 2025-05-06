using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagAddedEntityParticleEmitter : MonoBehaviour, IEmissionStrategy
{

    [Serializable]
    private class TagParticleEffectMapping
    {
        [SerializeField] ParticleSystem particleSystem;
        public ParticleSystem ParticleSystem => particleSystem;
        [SerializeField] TagScriptableObject tagScriptableObject;
        public TagScriptableObject TagScriptableObject => tagScriptableObject;
    }


    [SerializeField] List<TagParticleEffectMapping> mappings = new List<TagParticleEffectMapping>();
    [SerializeField] AnchorScriptableObject anchor;


    public void Emit(IEntity entity)
    {

        if (!mappings.Any(mapping => entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject))) return;

        foreach (var mapping in mappings)
        {
            if (entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject))
            {
                var position = entity.GetEntityComponent<IAnchoringComponent>().GetAnchor(anchor).position;
                var vfx = Instantiate(mapping.ParticleSystem, position, Quaternion.identity);
                vfx.Play();
            }
        }
    }
}

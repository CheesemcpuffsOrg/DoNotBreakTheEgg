using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;

public class ParticleBasedEntityEmission : EmissionBase<IEntity>
{

    [Serializable]
    private class TagParticleEffectMapping
    {
        [SerializeField] ParticleSystem particleSystem;
        public ParticleSystem ParticleSystem => particleSystem;
        [SerializeField] TagScriptableObject tagScriptableObject;
        public TagScriptableObject TagScriptableObject => tagScriptableObject;
        [SerializeField] AnchorScriptableObject anchor;
        public AnchorScriptableObject Anchor => anchor;

    }


    [SerializeField] List<TagParticleEffectMapping> mappings = new List<TagParticleEffectMapping>();

    [Header("Default Fall Back")]
    [SerializeField] ParticleSystem defaultParticleSystem;
    [SerializeField] AnchorScriptableObject anchor;

    public override void Emit(IEntity entity)
    {
        if (!mappings.Any(mapping => entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject)) && defaultParticleSystem != null && anchor != null)
        {
            var position = entity.GetEntityComponent<IAnchoringComponent>().GetAnchor(anchor).position;
            var vfx = Instantiate(defaultParticleSystem, position, Quaternion.identity);
            vfx.Play();
        }


        foreach (var mapping in mappings)
        {
            if (entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject))
            {
                var position = entity.GetEntityComponent<IAnchoringComponent>().GetAnchor(mapping.Anchor).position;
                var vfx = Instantiate(mapping.ParticleSystem, position, Quaternion.identity);
                vfx.Play();
            }
        }
    }
}

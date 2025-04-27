using ObservableCollections;
using R3;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ParticleEffectOnGrounded : MonoBehaviour
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

    [SerializeField] TagScriptableObject groundedTag;
    [SerializeField] AnchorScriptableObject anchor;

    // Start is called before the first frame update
    void Start()
    {
        EntityRegistry
            .RegisteredEntities
            .ObserveAdd()
            .Where(addEvent =>
                mappings.Any(mapping =>
                    addEvent.Value.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject)))
            .SelectMany(addEvent =>
            {
                var entity = addEvent.Value;
                var tagComponent = entity.GetEntityComponent<ITagComponent>();

                return Observable
                    .EveryUpdate()
                    .Select(_ => tagComponent.HasTag(groundedTag))
                    .DistinctUntilChanged() // Only emit when value changes
                    .Pairwise() // Get the previous and current value as a tuple
                    .Where(pair => !pair.Previous && pair.Current) 
                    .Select(_ => entity);
            })
            .Subscribe(entity =>
            {
                foreach (var mapping in mappings)
                {
                    if (entity.GetEntityComponent<ITagComponent>().HasTag(mapping.TagScriptableObject))
                    {
                        var position = entity.GetEntityComponent<IAnchoringComponent>().GetAnchor(anchor).position;
                        var vfx = Instantiate(mapping.ParticleSystem, position, Quaternion.identity);
                        vfx.Play();
                    }
                }
            });

    }

}

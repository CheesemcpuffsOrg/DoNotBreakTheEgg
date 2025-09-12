using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableComponent : MonoBehaviour, IBreakableComponent
{

    [SerializeField] ParticleSystem eggExplosion;

    [Header("Collisions")]
    [SerializeField] CollisionProxy collision;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isDeadTag;
    [SerializeField] TagFilter breakFilter;
    [SerializeField] TagFilter ignoreColisionsFilter;

    IEntity entity;

    private void Start()
    {
        entity = GetComponent<IEntity>();
    }

    private void CollisionEnter(Collision2D collision)
    {
        if (!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(breakFilter)) return;

        if(EntityCollisionService.TryGetEntity(collision.collider, out var collisionEntity))
        {
            if (collisionEntity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(ignoreColisionsFilter))
            {
                EntityCollisionService.IgnoreMainEntityColliders(entity, collisionEntity, true);
                return;
            }
        }

        entity.GetEntityComponent<ITagComponent>().AddTag(isDeadTag);

        var particles = Instantiate(eggExplosion, entity.GetEntityComponent<IAnchoringComponent>().GetPosition(), Quaternion.Euler(new Vector3(-90, 0, 0)), null);

        particles.Play();

        entity.Destroy(.01f);
    }

    private void OnEnable()
    {
        collision.OnCollisionEnter2D_Action += CollisionEnter;
    }

    private void OnDisable()
    {
        collision.OnCollisionEnter2D_Action -= CollisionEnter;
    }
}

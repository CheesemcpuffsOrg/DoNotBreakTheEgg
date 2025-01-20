using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEntityManager : MonoBehaviour
{
    public class ThrowData
    {

        private Vector2 velocity;
        public Vector2 Velocity => velocity;

        public float Gravity { get; }

        public Transform EntityTransform { get; }

        public ThrowData(Vector2 velocity, float gravity, Transform entityTransform)
        {
            this.velocity = velocity;
            Gravity = gravity;
            EntityTransform = entityTransform;
        }

        public void UpdateYVelocity(float yVelocity)
        {
            velocity.y = yVelocity;
        }
    }

    public static ThrowEntityManager Instance;

    Dictionary<IEntity, ThrowData> thrownEntities = new();

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        foreach(var thrownEntity in thrownEntities)
        {
            // Apply gravity

            var entityYVelocity = thrownEntity.Value.Velocity.y;

            thrownEntity.Value.UpdateYVelocity(entityYVelocity += thrownEntity.Value.Gravity * Time.fixedDeltaTime);

            // Update position
            thrownEntity.Value.EntityTransform.position += (Vector3)(thrownEntity.Value.Velocity * Time.fixedDeltaTime);
        }
    }

    public void AddEntityToThrow(IEntity entity, ThrowData throwData)
    {
        if (thrownEntities.ContainsKey(entity))
        {
            return;
        }

        thrownEntities.Add(entity, throwData);
    }

    public void RemoveThrownEntity(IEntity entity)
    {
        if(thrownEntities.ContainsKey(entity))
        {
            thrownEntities.Remove(entity);
        }
    }

    public bool IsEntityBeingThrown(IEntity entity)
    {
        if(thrownEntities.ContainsKey(entity))
            return true;

        return false;
    }


}

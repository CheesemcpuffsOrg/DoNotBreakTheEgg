using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEntityManager : MonoBehaviour
{
    public class ThrowData
    {

        private Vector2 distance;
        public Vector2 Distance => distance;

        public float Gravity { get; }

        public Transform EntityTransform { get; }

        public ThrowData(Vector2 distance, float gravity, Transform entityTransform)
        {
            this.distance = distance;
            Gravity = gravity;
            EntityTransform = entityTransform;
        }

        public void UpdateYVelocity(float yVelocity)
        {
            distance.y = yVelocity;
        }
    }

    [SerializeField] TagFilter stopThrowingFilter;

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

            var throwData = thrownEntity.Value;

            var entityYVelocity = throwData.Distance.y;

            throwData.UpdateYVelocity(entityYVelocity += throwData.Gravity * Time.fixedDeltaTime);

            // Update position
            throwData.EntityTransform.position += (Vector3)(throwData.Distance * Time.fixedDeltaTime);

            if (stopThrowingFilter.PassTagFilterCheck(thrownEntity.Key.GetEntityComponent<IGameObjectComponent>().GetTransform()))
            {
                throwData.EntityTransform.position = throwData.EntityTransform.position;
            }
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

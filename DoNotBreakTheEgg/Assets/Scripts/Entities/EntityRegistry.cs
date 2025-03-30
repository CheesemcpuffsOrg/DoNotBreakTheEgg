using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityRegistry
{

    private static List<IEntity> registeredEntities = new List<IEntity>();

    public static event Action<IEntity> EntityRegistered;
    public static event Action<IEntity> EntityUnregistered;

    public static void RegisterEntity(IEntity entity)
    {
        registeredEntities.Add(entity);
        EntityRegistered?.Invoke(entity);
    }

    public static void UnregisterEntity(IEntity entity)
    {
        registeredEntities.Remove(entity);
        EntityUnregistered?.Invoke(entity);
    }

    public static IEntity[] RegisteredEntities()
    {
        return registeredEntities.ToArray();
    } 

}

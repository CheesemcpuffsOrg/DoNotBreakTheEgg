using ObservableCollections;
using System;
using System.Linq;

public static class EntityRegistry
{
    static readonly ObservableList<IEntity> registeredEntities = new ObservableList<IEntity>();

    public static ObservableList<IEntity> RegisteredEntities = registeredEntities; 

    //private static List<IEntity> registeredEntities = new List<IEntity>();

    public static event Action<IEntity> EntityRegistered;
    public static event Action<IEntity> EntityUnregistered;

    public static void RegisterEntity(IEntity entity)
    {
        if (registeredEntities.Contains(entity)) return;

        registeredEntities.Add(entity);
        //EntityRegistered?.Invoke(entity);
    }

    public static void UnregisterEntity(IEntity entity)
    {
        if (!registeredEntities.Contains(entity)) return;

        registeredEntities.Remove(entity);
        //EntityUnregistered?.Invoke(entity);
    }

    public static IEntity[] GetRegisteredEntities()
    {
        return registeredEntities.ToArray();
    } 

}

using ObservableCollections;
using System;
using System.Linq;

public static class EntityRegistry
{
    static readonly ObservableList<IEntity> registeredEntities = new ObservableList<IEntity>();

    public static readonly ObservableList<IEntity> RegisteredEntities = registeredEntities; 

    public static void RegisterEntity(IEntity entity)
    {
        if (registeredEntities.Contains(entity)) return;

        registeredEntities.Add(entity);
    }

    public static void UnregisterEntity(IEntity entity)
    {
        if (!registeredEntities.Contains(entity)) return;

        registeredEntities.Remove(entity);
    }

    public static IEntity[] GetRegisteredEntities()
    {
        return registeredEntities.ToArray();
    } 

}

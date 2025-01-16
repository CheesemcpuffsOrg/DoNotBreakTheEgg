using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoEntity : MonoBehaviour, IEntity
{

    List<IEntityComponent> entityComponents = new List<IEntityComponent>();

    private void Awake()
    {

        // Get all components attached to this GameObject
        Component[] allComponents = GetComponents<Component>();

        // Filter components that implement the desired interface
        
        foreach (Component component in allComponents)
        {
            if (component is IEntityComponent myInterface)
            {
                entityComponents.Add(myInterface);
            }
        }
    }

    public T GetEntityComponent<T>() where T : class, IEntityComponent
    {
        foreach (var component in entityComponents)
        {
            if (component is T targetComponent)
            {
                return targetComponent;
            }
        }
        return null;
    }
}

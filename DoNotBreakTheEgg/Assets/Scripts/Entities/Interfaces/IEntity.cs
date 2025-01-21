using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public T GetEntityComponent<T>() where T : class, IEntityComponent;

    public void RegisterEntityComponent(IEntityComponent entityComponent);
}

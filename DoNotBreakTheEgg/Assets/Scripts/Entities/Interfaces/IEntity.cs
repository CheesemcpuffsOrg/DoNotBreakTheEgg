using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public T GetEntityComponent<T>() where T : class, IEntityComponent;

    public void RegisterEntityComponent(IEntityComponent entityComponent);

    public void Destroy(float time = 0);

    public Observable<IEntity> Destroyed { get; }
}

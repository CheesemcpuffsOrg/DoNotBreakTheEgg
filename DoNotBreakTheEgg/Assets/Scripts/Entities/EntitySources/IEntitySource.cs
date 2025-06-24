using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntitySource
{
    public Observable<IEntity> Entities { get; }
    public Observable<IEntity> LostEntities { get; }
}

using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySource : SourceBase<IEntity>
{
    // Strongly-typed observable for subscribers who know it's IEntity
    public Observable<IEntity> GainedEntities => gainedSubject;
    public Observable<IEntity> LostEntities => lostSubject;

    // Optional: strongly typed list
    public List<IEntity> PassingEntities => passingSet;
}

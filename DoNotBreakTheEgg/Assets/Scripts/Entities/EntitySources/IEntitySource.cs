using R3;
using System.Collections.Generic;

public interface IEntitySource
{
    public Observable<IEntity> Entities { get; }
    public Observable<IEntity> LostEntities { get; }

    public List<IEntity> PassingEntitySet { get; }
}

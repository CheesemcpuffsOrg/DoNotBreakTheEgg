using R3;
using System.Collections.Generic;

public interface ISource
{
    public Observable<object> Gained { get; }
    public Observable<object> Lost { get; }

    public List<object> PassingSet { get; }
}

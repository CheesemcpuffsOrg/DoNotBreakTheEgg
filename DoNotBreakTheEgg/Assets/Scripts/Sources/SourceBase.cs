using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SourceBase<T> : MonoBehaviour, ISource
{
    // Internal strongly-typed subjects
    protected readonly Subject<T> gainedSubject = new();
    protected readonly Subject<T> lostSubject = new();

    protected readonly List<T> passingSet = new();

    // Expose object-based ISource properties
    public virtual Observable<object> Gained => gainedSubject.Select(e => (object)e);
    public virtual Observable<object> Lost => lostSubject.Select(e => (object)e);

    public virtual List<object> PassingSet => passingSet.ConvertAll(e => (object)e);


    // Strongly typed access for subclasses
    protected void EmitEntity(T obj)
    {
        passingSet.Add(obj);
        gainedSubject.OnNext(obj);
    }

    protected void RemoveEntity(T obj)
    {
        passingSet.Remove(obj);
        lostSubject.OnNext(obj);
    }

}

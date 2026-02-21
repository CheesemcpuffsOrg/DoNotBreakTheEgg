using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EmissionBase<T> : MonoBehaviour, IEmission
{
    public Type EmissionType => typeof(T);

    public abstract void Emit(T obj);

    void IEmission.Emit(object obj)
    {
        if (obj is T t)
            Emit(t);
        else
            LoggingUtility.EditorOnlyLogError($"EmissionStrategy<{typeof(T)}> received incompatible type: {obj?.GetType()}");
    }
}

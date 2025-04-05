using UnityEngine;
using R3;
using System;
using System.Collections.Generic;

public static class R3Extensions
{
    private static readonly Dictionary<GameObject, OnDestroyNotifier> Notifiers = new Dictionary<GameObject, OnDestroyNotifier>();

    public static Observable<T> TakeUntilDestroy<T>(this Observable<T> source, MonoBehaviour owner)
    {
        return source.TakeUntil(owner.OnDestroyedAsObservable());
    }

    public static Observable<Unit> OnDestroyedAsObservable(this MonoBehaviour owner)
    {
        if (!Notifiers.TryGetValue(owner.gameObject, out var notifier))
        {
            notifier = owner.gameObject.AddComponent<OnDestroyNotifier>();
            Notifiers[owner.gameObject] = notifier;
        }

        var subject = new Subject<Unit>();

        notifier.OnDestroyed = () =>
        {
            subject.OnNext(default);
            subject.OnCompleted();
        };

        return subject;
    }

    private class OnDestroyNotifier : MonoBehaviour
    {
        public Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
            Notifiers.Remove(gameObject); // Clean up the notifier when the GameObject is destroyed
        }
    }
}



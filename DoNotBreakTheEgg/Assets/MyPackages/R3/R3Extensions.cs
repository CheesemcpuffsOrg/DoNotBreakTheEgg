using UnityEngine;
using R3;
using System;
using System.Collections.Generic;

public static class R3Extensions
{
    public static Observable<T> TakeUntilDestroy<T>(this Observable<T> source, MonoBehaviour owner)
    {
        return source.TakeUntil(owner.OnDestroyedAsObservable());
    }

    public static Observable<Unit> OnDestroyedAsObservable(this MonoBehaviour owner)
    {
        var notifier = owner.GetComponent<OnDestroyNotifier>()
                    ?? owner.gameObject.AddComponent<OnDestroyNotifier>();

        return notifier.Destroyed;
    }

    private class OnDestroyNotifier : MonoBehaviour
    {
        private readonly Subject<Unit> _subject = new();

        public Observable<Unit> Destroyed => _subject;

        private void OnDestroy()
        {
            _subject.OnNext(default);
            _subject.OnCompleted();
        }
    }
}



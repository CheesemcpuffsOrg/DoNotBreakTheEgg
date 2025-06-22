using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStreams : MonoBehaviour
{
    public static SoundStreams instance;

    Subject<UniqueSoundID> soundPlayedSubject = new Subject<UniqueSoundID>();
    public Observable<UniqueSoundID> soundPlayed => soundPlayedSubject;

    Subject<UniqueSoundID> soundStoppedSubject = new Subject<UniqueSoundID>();
    public Observable<UniqueSoundID> soundStopped => soundStoppedSubject;

    Subject<(UniqueSoundID, SoundData)> startTimer = new Subject<(UniqueSoundID, SoundData)>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        var subscriptionBag = Disposable.CreateBuilder();

        startTimer
            .SelectMany(tuple =>
            {
                var (UUID, data) = tuple;

                return Observable
                    .Timer(TimeSpan.FromSeconds(data.Delay))
                    .Select(_ => tuple);
            })
            .Subscribe(tuple =>
            {
                var (UUID, data) = tuple;

                var playLocation = data.PlayLocation;

                if (playLocation == null)
                {
                    playLocation = transform;
                }

                AdvancedAudioSystemManager.Instance.PlaySound(data.AudioScriptableObject, UUID, playLocation, data.FollowTransform);

                soundPlayedSubject.OnNext(UUID);
            })
            .AddTo(ref subscriptionBag);


        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

    public UniqueSoundID PlaySound(SoundData data)
    {
        var UUID = new UniqueSoundID();

        if (data.AudioScriptableObject == null) return null;

        startTimer.OnNext((UUID, data));

        return UUID;
    }

    public bool IsSoundPlaying(SoundData data, UniqueSoundID UUID)
    {
        return AdvancedAudioSystemManager.Instance.IsSoundPlaying(data.AudioScriptableObject, UUID);
    }

    public void StopSound(SoundData data, UniqueSoundID UUID)
    {
        if (data.AudioScriptableObject == null) return; //fails quitely if empty

        AdvancedAudioSystemManager.Instance.StopSound(data.AudioScriptableObject, UUID);

        soundStoppedSubject.OnNext(UUID);
    }
}

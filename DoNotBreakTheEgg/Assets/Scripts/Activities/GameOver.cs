using UnityEngine;
using R3;
using System;

public class GameOver : MonoBehaviour
{
    [SerializeField] EntitySource entitySource;

    [SerializeField] GameObject endGameUI;

    IDisposable subscriptionBag;

    private void Start()
    {

        endGameUI.SetActive(false);

        var disposable1 = entitySource
            .GainedEntities
            .Select(_ =>
            {
                return Observable.Timer(TimeSpan.FromSeconds(2));
            })
            .Switch()
            .Subscribe(_ =>
            {
                endGameUI.SetActive(true);

                InputControllerManager.instance.DisableAllControllers();
            });

        subscriptionBag = Disposable.Combine(disposable1);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

using R3;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{

    [SerializeField] Button playButton;

    [SerializeField] SceneReferenceScriptableObject persistentScene;

    private IDisposable subscriptionBag; // use this if you have less than 8 disposables


    // Start is called before the first frame update
    void Start()
    {
       var disposable1 = playButton
            .onClick
            .AsObservable()
            .Subscribe(_ =>
            {
                SceneManagerService.LoadScene(persistentScene);
            });

        subscriptionBag = Disposable.Combine(disposable1);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

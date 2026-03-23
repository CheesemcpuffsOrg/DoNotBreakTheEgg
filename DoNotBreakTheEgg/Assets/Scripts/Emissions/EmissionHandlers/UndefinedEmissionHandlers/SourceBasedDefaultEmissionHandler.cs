using R3;
using System;
using UnityEngine;

public class SourceBasedDefaultEmissionHandler : MonoBehaviour
{
    [SerializeField] GameObject entitySourceObj;
    ISource entitySource => entitySourceObj.GetComponent<ISource>();


    [SerializeField] GameObject entityEmissionStrategyObj;

    IEmission entityEmissionStrategy => entityEmissionStrategyObj.GetComponent<IEmission>();

    IDisposable subscriptionBag;

    // Start is called before the first frame update
    void Start()
    {

        var disposable1 = entitySource
            .Gained
            .Subscribe(entity =>
            {
                entityEmissionStrategy.Emit(Unit.Default);
            });

        subscriptionBag = Disposable.Combine(disposable1);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }
}

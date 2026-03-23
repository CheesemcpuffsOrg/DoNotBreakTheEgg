using R3;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventDefaultEmission : EmissionBase<Unit>
{

    [SerializeField] UnityEvent unityEvent;

    public override void Emit(Unit obj)
    {
        unityEvent.Invoke();
    }
}

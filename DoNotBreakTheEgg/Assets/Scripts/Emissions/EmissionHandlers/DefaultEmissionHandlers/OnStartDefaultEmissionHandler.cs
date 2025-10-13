using R3;
using UnityEngine;

public class OnStartDefaultEmissionHandler : MonoBehaviour
{

    [SerializeField] EmissionBase<Unit> emissionObject;

    private IEmission _emission;

    private IEmission emission => _emission ??= emissionObject.GetComponent<IEmission>();


    // Start is called before the first frame update
    void Start()
    {
        emission.Emit(Unit.Default);
    }

    
}

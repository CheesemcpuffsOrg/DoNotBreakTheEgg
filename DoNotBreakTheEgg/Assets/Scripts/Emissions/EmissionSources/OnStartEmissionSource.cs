using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartEmissionSource : MonoBehaviour
{

    [SerializeField] GameObject emissionStrategyObj;

    private IEmissionStrategy emissionStrategy => emissionStrategyObj.GetComponent<IEmissionStrategy>();

    // Start is called before the first frame update
    void Start()
    {

        emissionStrategy.Emit();
    }

   
}

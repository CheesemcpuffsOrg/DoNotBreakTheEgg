using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GenerateInputActionAsset : MonoBehaviour, IGeneratedInputActionAsset
{
    public IInputActionCollection2 CreateNewGeneratedInputActionAsset()
    {
        return new Controls();
    }
}

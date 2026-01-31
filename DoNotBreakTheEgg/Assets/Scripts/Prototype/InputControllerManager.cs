using System.Collections.Generic;
using UnityEngine;

public class InputControllerManager : MonoBehaviour
{

    Dictionary<int, InputController> inputControllers = new();


    public static InputControllerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddController(int id, InputController controller)
    {
        inputControllers.TryAdd(id, controller);
    }

    public void DisableAllControllers()
    {
        foreach (var controller in inputControllers.Values)
        {
            controller.DisableAllInputs();
        }
    }
}

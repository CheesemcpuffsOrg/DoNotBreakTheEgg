using System.Collections.Generic;
using UnityEngine;

public class InputControllerManager : MonoBehaviour
{

    Dictionary<int, PlayerInputController> inputControllers = new();


    public static InputControllerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddController(int id, PlayerInputController controller)
    {
        inputControllers.TryAdd(id, controller);
    }

    public void EnablePlayerUIControls(int index)
    {
        if (inputControllers.ContainsKey(index))
        {
            inputControllers[index].EnableUIInputs();
        }
    }

    public void DisableAllPlayerControllers()
    {
        foreach (var controller in inputControllers.Values)
        {
            controller.DisablePlayerInputs();
        }
    }

    public void DisableAllControllers()
    {
        foreach (var controller in inputControllers.Values)
        {
            controller.DisableAllInputs();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ChangeSceneOnInput : MonoBehaviour
{
    private bool sceneChanging = false;

    private void OnEnable()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
    }

    private void OnDisable()
    {
        // Nothing needed here
    }

    private void OnAnyButtonPressed(InputControl control)
    {
        if (sceneChanging)
            return;

        sceneChanging = true;

        GetComponent<SimpleSceneLoader>().LoadScene();
    }
}
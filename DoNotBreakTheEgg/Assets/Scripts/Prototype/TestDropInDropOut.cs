using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestDropInDropOut : MonoBehaviour
{

    [SerializeField] string resetGameKeyboard = "<Keyboard>/enter";
    [SerializeField] string resetGameGamepad = "<Gamepad>/start";

    InputAction resetAction;

    bool reset;

    private void Start()
    {
        resetAction = new InputAction(binding: resetGameKeyboard);
        resetAction.AddBinding(resetGameGamepad);
        resetAction.started += ResetGame;

        resetAction.Enable();

        if (reset) return;

        UserDeviceMappingUtil.DeleteAllUsers();

        reset = true;



    }

    void ResetGame(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

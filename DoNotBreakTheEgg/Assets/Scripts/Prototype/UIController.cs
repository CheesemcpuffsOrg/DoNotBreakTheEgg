using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {

    }
}

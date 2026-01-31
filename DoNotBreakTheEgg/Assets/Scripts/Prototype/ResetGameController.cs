using UnityEngine;
using UnityEngine.InputSystem;

public class ResetGameController : MonoBehaviour
{
    public void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}

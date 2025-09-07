
public static class SceneManagerService
{
    public static void LoadScene(SceneReferenceScriptableObject scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.GetSceneName());
    }
}

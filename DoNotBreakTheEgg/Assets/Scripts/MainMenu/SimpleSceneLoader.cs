using R3;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSceneLoader : MonoBehaviour
{
    [SerializeField] SceneReferenceScriptableObject persistentScene;

    public void LoadScene()
    {
        SceneManagerService.LoadScene(persistentScene);
    }
}

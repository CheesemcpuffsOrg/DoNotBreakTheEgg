using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDataManager : MonoBehaviour
{
    [SerializeField] GlobalDataScriptableObject dataScriptableObject;

    private float gravity;

    public float Gravity => gravity;

    public static GlobalDataManager Instance;

    private void Awake()
    {
        Instance = this;

        gravity = Physics2D.gravity.y * dataScriptableObject.Gravity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField] AudioScriptableObject sound;
    [SerializeField] GameObject soundComponentObj;

    ISoundComponent soundComponent => soundComponentObj.GetComponent<ISoundComponent>();

    // Start is called before the first frame update
    void Start()
    {
        soundComponent.PlaySound(sound, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

//this may need a second pass if you can quit to main menu from game
public class CleanUpInputUsers : MonoBehaviour
{
    private void Awake()
    {
        PlayerDataStorage.Instance?.DestroyStorage();
    }
}

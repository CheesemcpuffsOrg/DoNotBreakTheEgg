using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataStorage : MonoBehaviour
{
    public static PlayerDataStorage Instance { get; private set; }

    private List<InputActionCollectionAndUserData> persistentUsers = new List<InputActionCollectionAndUserData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Make the user data persist across scenes (user data automatically deletes on scene change)
    }

    public void AddUser(InputActionCollectionAndUserData userData)
    {
        persistentUsers.Add(userData);
    }

    public void RemoveUser(int userID)
    {
        persistentUsers.RemoveAll(data => data.UserData.Id == userID);
    }

    public IReadOnlyList<InputActionCollectionAndUserData> GetUsers()
    {
        return persistentUsers;
    }

    public void DestroyStorage()
    {
        persistentUsers.Clear();
        Destroy(gameObject);
        Instance = null;
    }
}

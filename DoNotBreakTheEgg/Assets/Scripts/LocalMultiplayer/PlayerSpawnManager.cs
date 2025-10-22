using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerSpawnManager : MonoBehaviour
{
    [Serializable]
    private class PlayerPrefabMapping
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;

        public int PlayerId { get; private set; }

        public GameObject PlayerPrefab => playerPrefab;
        public Transform SpawnPoint => spawnPoint;
        public void SetPlayerId(int id) => PlayerId = id;
    }

    [SerializeField] List<PlayerPrefabMapping> playerPrefabs = new List<PlayerPrefabMapping>();
    [SerializeField] GameObject inputControllerPrefab;

    private void Start()
    {
        RecreateAllPlayers();
    }

    private void RecreateAllPlayers()
    {
        var userList = PlayerDataStorage.Instance.GetUsers();

        foreach (var data in userList)
        {
            var inputControllerObj = Instantiate(inputControllerPrefab);

            var inputController = inputControllerObj.GetComponent<InputController>();

            inputController.InitializeControls(data.UserInputActions);

            var entity = GetEntity(data);

            if (entity == null)
            {
                Debug.LogError("Entity in list is null");
                return;
            }

            inputControllerObj.GetComponent<InputHandler>().SetEntity(entity);

            entity.GetEntityComponent<IAimComponent>().SetInputDevice(data.UserData.Device);
        }
    }

    private IEntity GetEntity(InputActionCollectionAndUserData inputActionCollectionAndUser)
    {
        var slotIndex = inputActionCollectionAndUser.UserData.SlotIndex;
        var id = inputActionCollectionAndUser.UserData.Id;

        var playerPrefab = playerPrefabs[slotIndex];

        playerPrefab.SetPlayerId(id);
        return Instantiate(playerPrefab.PlayerPrefab, playerPrefab.SpawnPoint.position, Quaternion.identity).GetComponent<IEntity>();
    }
}

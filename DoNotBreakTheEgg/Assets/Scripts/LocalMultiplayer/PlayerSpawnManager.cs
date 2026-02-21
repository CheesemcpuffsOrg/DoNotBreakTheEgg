using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

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

    [Header("UI")]
    [SerializeField] GameObject root;
    [SerializeField] GameObject firstSelected;


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

            var inputController = inputControllerObj.GetComponent<PlayerInputController>();
            var inputSystemUIInputModule = inputControllerObj.GetComponent<InputSystemUIInputModule>();
            var multiplayerEventSystem = inputControllerObj.GetComponent <MultiplayerEventSystem>();

            inputController.InitializeControls(data.UserInputActions, inputSystemUIInputModule);

            //the UI should probably be handled elsewhere
            multiplayerEventSystem.playerRoot = root;
            multiplayerEventSystem.SetSelectedGameObject(null);//set to null for button highlighting bug???
            multiplayerEventSystem.SetSelectedGameObject(firstSelected);

            var entity = GetEntity(data);

            if (entity == null)
            {
                Debug.LogError("Entity in list is null");
                return;
            }

            inputControllerObj.GetComponent<InputHandler>().SetEntity(entity);

            entity.GetEntityComponent<IAimComponent>().SetInputDevice(data.UserData.Device);

            InputControllerManager.instance.AddController(data.UserData.Id, inputController);
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

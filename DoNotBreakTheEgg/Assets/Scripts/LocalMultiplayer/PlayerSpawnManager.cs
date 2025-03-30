using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{

    private class EntityControllerMapping
    {
        public InputController Controller { get; }

        public IEntity Entity { get; }

        public EntityControllerMapping(InputController controller, IEntity entity)
        {
            Controller = controller;
            Entity = entity;
        }
    }

    [Serializable]
    private class PlayerPrefabMapping
    {
        [SerializeField] GameObject playerPrefab;

        public GameObject PlayerPrefab => playerPrefab;

        public bool InUse { get; private set; }

        public int PlayerId { get; private set; }

        public void SetInUse(bool inUse)
        {
            InUse = inUse;
        }

        public void SetPlayerId(int id)
        {
            PlayerId = id;
        }
    }

    [SerializeField] List<PlayerPrefabMapping> playerPrefabs = new List<PlayerPrefabMapping>();
    [SerializeField] GameObject inputControllerPrefab;
    [SerializeField] Transform spawnpoint;

    Dictionary<int, EntityControllerMapping> userDataStorage = new();

    bool startCalled;

    private void Start()
    {
        OnStartOrEnable();
        startCalled = true;   
    }


    void SpawnPlayer(InputActionCollectionAndUserData inputActionCollectionAndUser)
    {
        var inputControllerObj = Instantiate(inputControllerPrefab);

        var inputController = inputControllerObj.GetComponent<InputController>();

        inputController.InitializeControls(inputActionCollectionAndUser.UserInputActions);
        
        var entity = GetEntity(inputActionCollectionAndUser);

        if(entity == null) 
        {
            Debug.LogError("Entity in list is null");
            return;
        }

        inputControllerObj.GetComponent<InputHandler>().SetEntity(entity);

        userDataStorage.Add(inputActionCollectionAndUser.UserData.Id, new EntityControllerMapping(inputController, entity));
    }

    private IEntity GetEntity(InputActionCollectionAndUserData inputActionCollectionAndUser)
    {

        foreach(var playerPrefab in playerPrefabs)
        {
            if (!playerPrefab.InUse)
            {
                var id = inputActionCollectionAndUser.UserData.Id;

                playerPrefab.SetInUse(true);
                playerPrefab.SetPlayerId(id);
                return Instantiate(playerPrefab.PlayerPrefab, spawnpoint.position, Quaternion.identity).GetComponent<IEntity>();
            }
        }

        return null;
    }

    void DespawnPlayer(int id)
    {
        if (!userDataStorage.TryGetValue(id, out var mapping)) return;

        foreach (var playerPrefab in playerPrefabs)
        {
            if (playerPrefab.PlayerId == id)
            {
                playerPrefab.SetInUse(false);
                playerPrefab.SetPlayerId(-1);
            }
        }

        Destroy(mapping.Controller.gameObject);

        mapping.Entity.Destroy();

        userDataStorage.Remove(id);
    }

    void OnStartOrEnable()
    {
        LocalPlayerCreationManager.Instance.UserCreated += SpawnPlayer;
        LocalPlayerCreationManager.Instance.UserDeleted += DespawnPlayer;
    }

    private void OnEnable()
    {
        if (!startCalled) return;

        OnStartOrEnable();
    }

    private void OnDisable()
    {
        LocalPlayerCreationManager.Instance.UserCreated -= SpawnPlayer;
        LocalPlayerCreationManager.Instance.UserDeleted -= DespawnPlayer;
    }
}

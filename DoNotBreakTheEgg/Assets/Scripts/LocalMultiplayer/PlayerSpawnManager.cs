using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> playerPrefabs = new List<GameObject>();
    [SerializeField] GameObject inputControllerPrefab;
    [SerializeField] Transform spawnpoint;

    List<(InputController, IEntity)> entityControllerPairing = new();

    bool startCalled;

    private void Start()
    {
        OnStartOrEnable();
        startCalled = true;
    }


    void SpawnPlayer(InputActionCollectionAndUser inputActionCollectionAndUser)
    {
        var inputControllerObj = Instantiate(inputControllerPrefab);

        var inputController = inputControllerObj.GetComponent<InputController>();

        inputController.InitializeControls(inputActionCollectionAndUser.UserInputActions);

        var player = Instantiate(playerPrefabs[inputActionCollectionAndUser.User.index], spawnpoint.position, Quaternion.identity);

        var entity = player.GetComponent<IEntity>();

        inputControllerObj.GetComponent<InputHandler>().SetEntity(entity);

        entityControllerPairing.Add((inputController, entity));
    }

    void DespawnPlayer(int index)
    {
        var tuple = entityControllerPairing[index];

        entityControllerPairing.RemoveAt(index);

        Destroy(tuple.Item1.gameObject);

        tuple.Item2.GetEntityComponent<IGameObjectComponent>().Destroy();
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

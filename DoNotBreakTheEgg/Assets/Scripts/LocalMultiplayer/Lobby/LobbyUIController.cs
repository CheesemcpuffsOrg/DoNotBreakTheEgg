using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyUIController : MonoBehaviour
{
    [Serializable]
    private class UIPanelMapping
    {
        [SerializeField] GameObject playerPanel;

        public GameObject PlayerPanel => playerPanel;

        [SerializeField] GameObject playerPrompt;

        public GameObject PlayerPrompt => playerPrompt;
    }

    [SerializeField] List<UIPanelMapping> panelmappings = new List<UIPanelMapping>();

    Dictionary<int, UIPanelMapping> playerPanelMapping = new Dictionary<int, UIPanelMapping>();

    bool startCalled;


    private void Start()
    {
        OnStartOrEnable();

        startCalled = true;

        foreach (var mapping in panelmappings)
        {
            mapping.PlayerPanel.SetActive(false);
            mapping.PlayerPrompt.SetActive(true);
        }
    }

   

    private void EnableUI(InputActionCollectionAndUserData data)
    {
        var playerIndex = data.UserData.Index;
        var playerID = data.UserData.Id;

        var mapping = panelmappings[playerIndex];

        mapping.PlayerPanel.SetActive(true);
        mapping.PlayerPrompt.SetActive(false);

        playerPanelMapping.TryAdd(playerID, panelmappings[playerIndex]);
    }

    private void DisableUI(int playerID)
    {
        var mapping = playerPanelMapping[playerID];

        mapping.PlayerPanel.SetActive(false);
        mapping.PlayerPrompt.SetActive(true);

        playerPanelMapping.Remove(playerID);    
    }

    void OnStartOrEnable()
    {
        LocalPlayerCreationManager.Instance.UserCreated += EnableUI;
        LocalPlayerCreationManager.Instance.UserDeleted += DisableUI;
    }

    private void OnEnable()
    {
        if (!startCalled) return;

        OnStartOrEnable();
    }

    private void OnDisable()
    {
        LocalPlayerCreationManager.Instance.UserCreated -= EnableUI;
        LocalPlayerCreationManager.Instance.UserDeleted -= DisableUI;
    }
}

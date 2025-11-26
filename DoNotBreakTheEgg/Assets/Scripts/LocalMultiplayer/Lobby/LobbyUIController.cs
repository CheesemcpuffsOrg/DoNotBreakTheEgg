using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [Serializable]
    private class UIPanelMapping
    {
        [SerializeField] GameObject playerPanel;

        public GameObject PlayerPanel => playerPanel;

        [SerializeField] GameObject playerPrompt;

        public GameObject PlayerPrompt => playerPrompt; 

        public bool InUse {  get; set; }
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
            mapping.InUse = false;
            mapping.PlayerPanel.SetActive(false);
            mapping.PlayerPrompt.SetActive(true);
        }
    }

   

    private void EnableUI(InputActionCollectionAndUserData data)
    {
        var playerID = data.UserData.Id;

        var mapping = panelmappings.Find(m => !m.InUse);

        if (mapping == null)
        {
            Debug.LogError("No available panels for new player!");
            return;
        }

        // Assign and mark as used
        mapping.InUse = true;
        playerPanelMapping[playerID] = mapping;

        mapping.PlayerPanel.SetActive(true);
        mapping.PlayerPrompt.SetActive(false);
    }

    private void DisableUI(int playerID)
    {
        if (!playerPanelMapping.TryGetValue(playerID, out var mapping))
        {
            Debug.LogError($"No panel mapping found for PlayerID {playerID}");
            return;
        }

        // Free and reset the panel
        mapping.InUse = false;
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

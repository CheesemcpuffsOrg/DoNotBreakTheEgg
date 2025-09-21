using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDataStorage 
{
    public class PlayerData
    {
        private int playerIndex;

        public int PlayerIndex => playerIndex;

        public PlayerData(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }

    private static Dictionary<int, InputActionCollectionAndUserData> activePlayerData = new();

    public static void StorePlayerData(int playerID, InputActionCollectionAndUserData data)
    {
        activePlayerData.TryAdd(playerID, data);
    }

    public static void RemovePlayerData(int playerID)
    {
        activePlayerData.Remove(playerID);
    }

    public static Dictionary<int, InputActionCollectionAndUserData> GetAllPlayerData()
    {
        return activePlayerData;
    }
}

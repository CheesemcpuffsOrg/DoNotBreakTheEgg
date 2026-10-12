using System.Collections.Generic;
using UnityEngine;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance;

    Dictionary<IEntity, int> playerIdEntityMapping = new Dictionary<IEntity, int>();

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }

    }

    public void SetPlayerIdEntityMapping(int playerId, IEntity entity)
    {
        playerIdEntityMapping.TryAdd(entity, playerId);
    }

    public int GetEntityPlayerId(IEntity entity)
    {
        return playerIdEntityMapping[entity];
    }
}

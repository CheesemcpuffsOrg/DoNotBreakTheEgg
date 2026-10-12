using System.Threading.Tasks;
using UnityEngine;

public class QuickTimeEvent : MonoBehaviour
{

    public static QuickTimeEvent Instance;

    private void Awake()
    {
        // Check if there's already an instance of this class
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Another instance of the singleton {nameof(QuickTimeEvent)} exists. Please make sure there is only one");
            return;
        }

        Instance = this;
    }

    //subscribe separately to button calls
    //when they are all pressed flip a boolean

    public async Task<bool> StartEvent(IEntity entity)
    {
        Time.timeScale = 0.2f;

        bool success = false;

        var playerId = PlayerRegistry.Instance.GetEntityPlayerId(entity);

        InputControllerManager.instance.EnablePlayerQuickTimeEventControls(playerId);

        // Start listening for the required input
        // PSEUDO:
        // OnInput += () => success = true;

        // Give the player 3 REAL seconds
        for (int i = 0; i < 300; i++)
        {
            await Task.Delay(10);

            // PSEUDO:
            // if (InputHappened)
            // {
            //     success = true;
            //     break;
            // }
        }

        Time.timeScale = 1f;

        return success;
    }
}

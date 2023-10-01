using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public const int NUMBER_OF_PLAYERS = 2;
    private const string PLAYER_1_DEVICE_NAME = "DualShock4GamepadHID";
    private const string PLAYER_2_DEVICE_NAME = "SwitchProControllerHID";

    public delegate void InputEnabledHandler(bool newEnabled);
    public static InputEnabledHandler InputEnabled;

    private delegate void GameplayInputsPausedHandler(bool newGameplayInputsPaused);
    private static GameplayInputsPausedHandler GameplayInputsPaused;


    [Header("Components")]
    [SerializeField]
    private PlayerInputManager playerInputManager;
    [SerializeField]
    private GameObject playerPrefab;

    private bool inputsPaused = false;
    private PlayerControls[] playerControls;

    public static void PauseGameplayInputs(bool newGameplayInputsPaused)
    {
        if (GameplayInputsPaused != null)
        {
            GameplayInputsPaused(newGameplayInputsPaused);
        }
    }


    void Awake()
    {
        InputManager.GameplayInputsPaused += PauseInputs;

        // playerControls = new PlayerControls();
        // playerControls.Enable();
        // playerInputs = new PlayerInput[NUMBER_OF_PLAYERS];

        // Debug.Log("Join Both players here");
        // playerInputManager.playerPrefab = playerPrefab;
        // playerInputManager.JoinPlayer(0, 0, playerControls.GamepadScheme.name, InputSystem.GetDevice(PLAYER_1_DEVICE_NAME));
        // playerInputManager.JoinPlayer(1, 1, playerControls.GamepadScheme.name, InputSystem.GetDevice(PLAYER_2_DEVICE_NAME));
    }


    private void PauseInputs(bool newInputsPaused)
    {
        inputsPaused = newInputsPaused;

        if (InputEnabled != null)
        {
            InputEnabled(!inputsPaused);
        }
    }


    public void OnPlayerJoinedEvent(PlayerInput playerInput)
    {
        Debug.Log("Player joined");
    }

    public void OnPlayerLeftEvent(PlayerInput playerInput)
    {
        Debug.Log("Player left");
    }
}

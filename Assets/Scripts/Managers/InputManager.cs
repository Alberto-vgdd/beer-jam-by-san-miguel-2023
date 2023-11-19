using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public static int NUMBER_OF_PLAYERS = 1;
    private const string PLAYER_1_DEVICE_NAME = "DualShock4GamepadHID";
    private const string PLAYER_2_DEVICE_NAME = "SwitchProControllerHID";

    public delegate void InputEnabledHandler(bool newEnabled);
    public static InputEnabledHandler[] InputEnabled = new InputEnabledHandler[NUMBER_OF_PLAYERS];


    [Header("Parameters")]
    [SerializeField]
    private bool developmentMode = false;
    [SerializeField]
    private string[] playerDeviceNames = { PLAYER_1_DEVICE_NAME, PLAYER_2_DEVICE_NAME };

    private bool inputsPaused = false;
    private PlayerControls[] playerControls;

    private int winnerPlayerNumber;
    public PlayerControls WinnerPlayerControls { get => playerControls[winnerPlayerNumber]; }
    public int WinnerPlayerNumber { get => winnerPlayerNumber; set => winnerPlayerNumber = value; }

    protected override void Awake()
    {
        base.Awake();

        if (developmentMode)
        {
            foreach (InputDevice inputDevice in InputSystem.devices)
            {
                Debug.Log(inputDevice.name);
            }
        }

        playerControls = new PlayerControls[NUMBER_OF_PLAYERS];
        for (int playerNumber = 0; playerNumber < NUMBER_OF_PLAYERS; playerNumber++)
        {
            playerControls[playerNumber] = new PlayerControls();
            if (!developmentMode)
            {
                // InputDevice inputDevice = InputSystem.GetDevice(playerDeviceNames[playerNumber]);
                // playerControls[playerNumber].devices = new InputDevice[1] { inputDevice };
            }
        }
    }


    public void PauseInputs(int playerNumber, bool newInputsPaused)
    {
        inputsPaused = newInputsPaused;
        InputEnabled[playerNumber]?.Invoke(!inputsPaused);
    }

    protected override InputManager GetThis()
    {
        return this;
    }

    internal PlayerControls GetPlayerControls(int playerId)
    {
        return playerControls[playerId];
    }
}

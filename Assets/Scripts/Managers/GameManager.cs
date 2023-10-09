using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private DifficultyManager difficultyManager;

    private PlayerControls playerControls;

    [SerializeField]
    private PlayerTable[] playerTables;

    private bool[] arePlayersGameOver;
    private PlayerProgress[] playersGameProgresses;

    protected override void Awake()
    {
        base.Awake();

        arePlayersGameOver = new bool[InputManager.NUMBER_OF_PLAYERS];
        playersGameProgresses = new PlayerProgress[InputManager.NUMBER_OF_PLAYERS];

        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    void Start()
    {
        uIManager.ShowTitleScreen();
    }

    void OnEnable()
    {
        playerControls.Navigation.ExitGame.performed += OnExitGameButtonPressed;
    }

    void OnDisable()
    {
        playerControls.Navigation.ExitGame.performed -= OnExitGameButtonPressed;
    }

    private void OnGameOver(PlayerProgress playerProgress)
    {
        int playerNumber = playerProgress.playerNumber;

        DifficultyManager.GameOver[playerNumber] -= OnGameOver;
        InputManager.Instance.PauseInputs(playerNumber, true);

        arePlayersGameOver[playerNumber] = true;
        playersGameProgresses[playerNumber] = playerProgress;
        playerTables[playerNumber].StopGame();

        bool allPlayersAreGameOver = true;
        foreach (bool isPlayerGameOver in arePlayersGameOver)
        {
            allPlayersAreGameOver &= isPlayerGameOver;
        }

        if (allPlayersAreGameOver)
        {
            // TODO : FIX UI.
            int winnerPlayerNumber = GetWinnerPlayerNumber();
            uIManager.ShowGameOverScreen(winnerPlayerNumber, playersGameProgresses);
        }
    }

    public void StartNewGame()
    {
        uIManager.ShowGameplayScreen();

        foreach (PlayerTable playerTable in playerTables)
        {
            int playerNumber = playerTable.PlayerNumber;

            playerTable.StartGame();
            arePlayersGameOver[playerNumber] = false;
            playersGameProgresses[playerNumber] = null;
            DifficultyManager.GameOver[playerNumber] += OnGameOver;
            InputManager.Instance.PauseInputs(playerNumber, false);
        }

        difficultyManager.ResetProgress();
    }

    private void OnExitGameButtonPressed(InputAction.CallbackContext context)
    {
        ExitGame();
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    private int GetWinnerPlayerNumber()
    {
        int highestScore = 0;
        int playerWithHighestScore = 0;

        foreach (PlayerProgress playerProgress in playersGameProgresses)
        {
            if (playerProgress.totalScore > highestScore)
            {
                highestScore = playerProgress.totalScore;
                playerWithHighestScore = playerProgress.playerNumber;
            }
        }

        return playerWithHighestScore;
    }


    protected override GameManager GetThis()
    {
        return this;
    }

}

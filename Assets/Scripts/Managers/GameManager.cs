using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private DifficultyManager difficultyManager;
    [SerializeField]
    private CameraManager cameraManager;



    private PlayerControls playerControls;
    private PlayerTable[] playerTables;
    private bool[] arePlayersGameOver;
    private PlayerProgress[] playersGameProgresses;



    protected override void Awake()
    {
        base.Awake();

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

    public void StartNewGame(int numberOfPlayers)
    {
        StartCoroutine(LoadGameFor(numberOfPlayers));
    }

    private IEnumerator LoadGameFor(int numberOfPlayers)
    {
        InputManager.NUMBER_OF_PLAYERS = numberOfPlayers;

        arePlayersGameOver = new bool[numberOfPlayers];
        playersGameProgresses = new PlayerProgress[numberOfPlayers];
        playerTables = new PlayerTable[numberOfPlayers];

        PlayerTable.PlayerJoined += OnPlayerJoined;

        yield return SceneManager.LoadSceneAsync(numberOfPlayers, LoadSceneMode.Additive);

        PlayerTable.PlayerJoined -= OnPlayerJoined;

        StartGame();
    }

    private void StartGame()
    {
        PieceManager.Instance.OnGameStarted();
        cameraManager.FrameGameplayArea();
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

    private void OnPlayerJoined(int playerNumber, PlayerTable playerTable)
    {
        playerTable.SetPlayerControls(InputManager.Instance.GetPlayerControls(playerNumber));
        playerTables[playerNumber] = playerTable;

    }

    public void PlayAgain()
    {
        StartGame();
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
            PieceManager.Instance.OnGameFinished();
            int winnerPlayerNumber = GetWinnerPlayerNumber();
            uIManager.ShowGameOverScreen(winnerPlayerNumber, playersGameProgresses);
        }
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


    private void OnExitGameButtonPressed(InputAction.CallbackContext context)
    {
        ExitGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoBackToMainMenu()
    {
        StartCoroutine(UnloadGameFor(InputManager.NUMBER_OF_PLAYERS));
    }
    private IEnumerator UnloadGameFor(int numberOfPlayers)
    {
        yield return SceneManager.UnloadSceneAsync(numberOfPlayers);
        uIManager.ShowTitleScreen();

    }

    protected override GameManager GetThis()
    {
        return this;
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private DifficultyManager difficultyManager;
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private LeaderboardsManager leaderboardsManager;

    [Header("Paramerters")]
    [SerializeField]
    private bool enableLeaderboardSystem;

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
        ShowTitleScreen();
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
        PlayerControls playerJoinedControls = InputManager.Instance.GetPlayerControls(playerNumber);
        playerJoinedControls.Enable();
        playerTable.SetPlayerControls(playerJoinedControls);
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
            int winnerPlayerScore = GetPlayerScore(winnerPlayerNumber);
            InputManager.Instance.WinnerPlayerNumber = winnerPlayerNumber;
            bool isNewRecord = false;

            if (enableLeaderboardSystem)
            {
                isNewRecord = leaderboardsManager.CheckForNewScore(winnerPlayerScore) >= 0;

                if (isNewRecord)
                {
                    RestrictUIInputsTo(winnerPlayerNumber);
                }
            }

            uIManager.ShowGameOverScreen(winnerPlayerNumber, playersGameProgresses, isNewRecord);
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

    private int GetPlayerScore(int playerNumber)
    {
        return playersGameProgresses[playerNumber].totalScore;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoBackToMainMenu()
    {
        UnloadGameInTheBackground();
        ShowTitleScreen();
    }

    private void ShowTitleScreen()
    {
        RestrictUIInputsTo(0);
        uIManager.ShowTitleScreen();
    }
    private void RestrictUIInputsTo(int playerNumber)
    {
        playerControls.devices = InputManager.Instance.GetPlayerControls(playerNumber).devices;
        UIManager.Instance.OnlyReadInputsFrom(playerControls);
    }

    internal void UnloadGameInTheBackground()
    {
        StartCoroutine(UnloadGameFor(InputManager.NUMBER_OF_PLAYERS));

    }
    private IEnumerator UnloadGameFor(int numberOfPlayers)
    {
        for (int playerNumber = 0; playerNumber < numberOfPlayers; playerNumber++)
        {
            PlayerControls playerLeftControls = InputManager.Instance.GetPlayerControls(playerNumber);
            playerLeftControls.Disable();
        }

        yield return SceneManager.UnloadSceneAsync(numberOfPlayers);
    }

    protected override GameManager GetThis()
    {
        return this;
    }

}

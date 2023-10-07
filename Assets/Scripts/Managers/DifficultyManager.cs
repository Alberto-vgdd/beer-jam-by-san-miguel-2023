using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public delegate void DifficultyChangedHandler(float newDifficulty, int newDisplayLevelNumber);
    public static DifficultyChangedHandler[] PlayerDifficultyChanged = new DifficultyChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void BoxesCompletedLeftChangedHandler(int newBoxesCompletedLeft);
    public static BoxesCompletedLeftChangedHandler[] PlayerBoxesCompletedLeftChanged = new BoxesCompletedLeftChangedHandler[InputManager.NUMBER_OF_PLAYERS];


    public delegate void ScoreChangedHandler(int newScore);
    public static ScoreChangedHandler[] PlayerScoreChanged = new ScoreChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void LifesLeftChangedHandler(int newLifesLefts);
    public static LifesLeftChangedHandler[] PlayerLifesLeftChanged = new LifesLeftChangedHandler[InputManager.NUMBER_OF_PLAYERS];


    public delegate void GameOverHandler(int playerNumber, int newScore);
    public static GameOverHandler GameOver;

    private const int MAX_LIVES = 3;

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 9)]
    private int levelNumber = 0;
    [SerializeField]
    private int[] levelToObjectiveBoxesCount = new int[10];
    [SerializeField]
    private int[] boxesCompletedToMultiplier;
    private float difficultyIncrease = 1 / 9f;
    private int scorePerBoxCompleted = 100;


    private PlayerProgress[] playersProgresses;


    void Awake()
    {
        playersProgresses = new PlayerProgress[InputManager.NUMBER_OF_PLAYERS];
        for (int playerNumber = 0; playerNumber < InputManager.NUMBER_OF_PLAYERS; playerNumber++)
        {
            playersProgresses[playerNumber] = new PlayerProgress(playerNumber);
        }
    }

    void OnEnable()
    {
        PlayerTable.BeerBoxCompleted += OnBeerBoxCompleted;
        PlayerTable.BeerBoxRuined += OnBeerBoxRuined;
    }

    void OnDisable()
    {
        PlayerTable.BeerBoxCompleted -= OnBeerBoxCompleted;
        PlayerTable.BeerBoxRuined -= OnBeerBoxRuined;
    }

    private void OnPlayerProgressLevelChanged(PlayerProgress playerProgress)
    {
        playerProgress.completedBoxes = 0;
        playerProgress.objectiveCompletedBoxes = levelToObjectiveBoxesCount[levelNumber];
        playerProgress.difficulty = difficultyIncrease * levelNumber;
        playerProgress.livesLeft = Mathf.Min(MAX_LIVES, playerProgress.livesLeft + 1);


        if (PlayerDifficultyChanged[playerProgress.playerNumber] != null)
        {
            PlayerDifficultyChanged[playerProgress.playerNumber](playerProgress.difficulty, playerProgress.levelNumber + 1);
        }

        if (PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber] != null)
        {
            PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber](playerProgress.objectiveCompletedBoxes);
        }

        if (PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber] != null)
        {
            PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber](playerProgress.totalScore);
        }

        if (PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber] != null)
        {
            PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber](playerProgress.livesLeft);
        }
    }

    private void OnBeerBoxCompleted(int playerNumber, int newBoxesCompleted)
    {
        PlayerProgress playerProgress = playersProgresses[playerNumber];

        int boxesCompleted = boxesCompletedToMultiplier[Mathf.Clamp(newBoxesCompleted, 1, boxesCompletedToMultiplier.Length) - 1];
        playerProgress.completedBoxes += boxesCompleted;
        playerProgress.totalScore += boxesCompleted * scorePerBoxCompleted;

        PlayerBoxesCompletedLeftChanged[playerNumber]?.Invoke(Mathf.Max(0, playerProgress.objectiveCompletedBoxes - playerProgress.completedBoxes));
        PlayerScoreChanged[playerNumber]?.Invoke(playerProgress.totalScore);

        if (playerProgress.completedBoxes >= playerProgress.objectiveCompletedBoxes && playerProgress.levelNumber < 9)
        {
            playerProgress.levelNumber++;
            OnPlayerProgressLevelChanged(playerProgress);
        }

    }

    private void OnBeerBoxRuined(int playerNumber)
    {
        PlayerProgress playerProgress = playersProgresses[playerNumber];

        CameraManager.ShakeCamera(0.2f, 0.15f, 2);

        playerProgress.livesLeft = Mathf.Max(0, playerProgress.livesLeft - 1);
        PlayerLifesLeftChanged[playerNumber]?.Invoke(playerProgress.livesLeft);

        if (playerProgress.livesLeft <= 0 && GameOver != null)
        {
            GameOver(playerNumber, playerProgress.totalScore);
        }
    }

    internal void ResetProgress()
    {
        foreach (PlayerProgress playerProgress in playersProgresses)
        {
            playerProgress.Reset();
            OnPlayerProgressLevelChanged(playerProgress);
        }
    }
}

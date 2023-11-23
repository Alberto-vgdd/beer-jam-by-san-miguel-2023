using System;
using System.Collections;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public delegate void DifficultyChangedHandler(float newDifficulty, int newDisplayLevelNumber);
    public static DifficultyChangedHandler[] PlayerDifficultyChanged = new DifficultyChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void DifficultyChangedByPowerUpHandler(float newDifficulty);
    public static DifficultyChangedByPowerUpHandler[] PlayerDifficultyChangedByPowerUp = new DifficultyChangedByPowerUpHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void BoxesCompletedLeftChangedHandler(int newBoxesCompletedLeft);
    public static BoxesCompletedLeftChangedHandler[] PlayerBoxesCompletedLeftChanged = new BoxesCompletedLeftChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void ScoreChangedHandler(int newScore);
    public static ScoreChangedHandler[] PlayerScoreChanged = new ScoreChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void LifesLeftChangedHandler(int newLifesLefts);
    public static LifesLeftChangedHandler[] PlayerLifesLeftChanged = new LifesLeftChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    public delegate void GameOverHandler(PlayerProgress playerProgress);
    public static GameOverHandler[] GameOver = new GameOverHandler[InputManager.NUMBER_OF_PLAYERS];


    public delegate void BeerBoxPowerUpHandler(float time, int increase);
    public static BeerBoxPowerUpHandler[] BeerBoxPowerUp = new BeerBoxPowerUpHandler[InputManager.NUMBER_OF_PLAYERS];


    private const int MAX_LIVES = 3;

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 9)]
    private int startLevelNumber = 0;
    [SerializeField]
    private int[] levelToObjectiveBoxesCount = new int[10];
    [SerializeField]
    private int[] boxesCompletedToMultiplier;
    private float difficultyIncrease = 1 / 9f;
    private int scorePerBoxCompleted = 100;


    private PlayerProgress[] playersProgresses;

    Coroutine[] powerUpCoroutine = new Coroutine[2];

    bool[] powerUpActivated = new bool[2] { false, false };

    bool isPowerUpDiff = false;



    void Awake()
    {
        playersProgresses = new PlayerProgress[InputManager.NUMBER_OF_PLAYERS];
        for (int playerNumber = 0; playerNumber < InputManager.NUMBER_OF_PLAYERS; playerNumber++)
        {
            playersProgresses[playerNumber] = new PlayerProgress(playerNumber);
        }
    }

    void Start()
    {
        ResetProgress();
    }

    void OnEnable()
    {
        PlayerTable.BeerBoxCompleted += OnBeerBoxCompleted;
        PlayerTable.BeerBoxRuined += OnBeerBoxRuined;
        PlayerTable.BeerBoxPowerUp += OnBeerBoxPowerUp;
    }

    void OnDisable()
    {
        PlayerTable.BeerBoxCompleted -= OnBeerBoxCompleted;
        PlayerTable.BeerBoxRuined -= OnBeerBoxRuined;
        PlayerTable.BeerBoxPowerUp -= OnBeerBoxPowerUp;
    }

    private void OnPlayerProgressLevelChanged(PlayerProgress playerProgress)
    {
        playerProgress.completedBoxes = 0;
        playerProgress.objectiveCompletedBoxes = levelToObjectiveBoxesCount[playerProgress.levelNumber];
        if (!powerUpActivated[playerProgress.playerNumber]) 
        {
            playerProgress.difficulty = difficultyIncrease * playerProgress.levelNumber;
            isPowerUpDiff = false;
        }
        if (!isPowerUpDiff)
        {
            playerProgress.livesLeft = Mathf.Min(MAX_LIVES, playerProgress.livesLeft + 1);
        }


        PlayerDifficultyChanged[playerProgress.playerNumber]?.Invoke(playerProgress.difficulty, playerProgress.levelNumber + 1);
        PlayerBoxesCompletedLeftChanged[playerProgress.playerNumber]?.Invoke(playerProgress.objectiveCompletedBoxes);
        PlayerScoreChanged[playerProgress.playerNumber]?.Invoke(playerProgress.totalScore);
        PlayerLifesLeftChanged[playerProgress.playerNumber]?.Invoke(playerProgress.livesLeft);
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

        if (playerProgress.livesLeft <= 0)
        {
            GameOver[playerNumber]?.Invoke(playerProgress);
        }
    }

    private void OnBeerBoxPowerUp(int playerNumber, int increase) 
    {
        isPowerUpDiff = true;
        PlayerProgress playerProgress = playersProgresses[playerNumber];
        if (increase > 0)
        {
            PlayerDifficultyChanged[playerProgress.playerNumber]?.Invoke(playerProgress.difficulty * 2, playerProgress.levelNumber + 1);

        }
        else 
        {
            PlayerDifficultyChanged[playerProgress.playerNumber]?.Invoke(increase, playerProgress.levelNumber + 1);
        }

        int time = 10;
        powerUpActivated[playerNumber] = true;

        BeerBoxPowerUp[playerNumber]?.Invoke(time, increase);

        if (powerUpCoroutine[playerNumber] != null) 
        {
            StopCoroutine(powerUpCoroutine[playerNumber]);
        }

        powerUpCoroutine[playerNumber] = StartCoroutine(BeerBoxRestorePowerUp(playerNumber, time));
    }

    IEnumerator BeerBoxRestorePowerUp(int playerNumber, float time) 
    {

        yield return new WaitForSeconds(time);



        PlayerProgress playerProgress = playersProgresses[playerNumber];
        playerProgress.difficulty = difficultyIncrease * playerProgress.levelNumber;

        PlayerDifficultyChanged[playerProgress.playerNumber]?.Invoke(playerProgress.difficulty, playerProgress.levelNumber+1);
        powerUpActivated[playerNumber] = false;
    }

    internal void ResetProgress()
    {
        foreach (PlayerProgress playerProgress in playersProgresses)
        {
            playerProgress.Reset();
            playerProgress.levelNumber = startLevelNumber;
            OnPlayerProgressLevelChanged(playerProgress);
        }
    }
}

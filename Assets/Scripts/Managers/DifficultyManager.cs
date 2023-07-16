using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public delegate void DifficultyChangedHandler(float newDifficulty, int newDisplayLevelNumber);
    public static DifficultyChangedHandler DifficultyChanged;

    public delegate void BoxesCompletedLeftChangedHandler(int newBoxesCompletedLeft);
    public static BoxesCompletedLeftChangedHandler BoxesCompletedLeftChanged;

    public delegate void ScoreChangedHandler(int newScore);
    public static ScoreChangedHandler ScoreChanged;

    public delegate void LifesLeftChangedHandler(int newLifesLefts);
    public static LifesLeftChangedHandler LifesLeftChanged;


    public delegate void GameOverHandler(int newScore);
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



    private float difficulty = 0;
    int completedBoxes = 0;
    int objectiveCompletedBoxes = 0;
    int totalScore = 0;
    int livesLeft = 3;


    void Start()
    {
        OnLevelChanged();
    }

    void OnEnable()
    {
        Level.BeerBoxCompleted += OnBeerBoxCompleted;
        Level.BeerBoxRuined += OnBeerBoxRuined;
    }

    void OnDisable()
    {
        Level.BeerBoxCompleted -= OnBeerBoxCompleted;
        Level.BeerBoxRuined -= OnBeerBoxRuined;
    }

    private void OnLevelChanged()
    {
        completedBoxes = 0;
        objectiveCompletedBoxes = levelToObjectiveBoxesCount[levelNumber];
        difficulty = difficultyIncrease * levelNumber;
        livesLeft = Mathf.Min(MAX_LIVES, livesLeft + 1);


        if (DifficultyChanged != null)
        {
            DifficultyChanged(difficulty, levelNumber + 1);
        }

        if (BoxesCompletedLeftChanged != null)
        {
            BoxesCompletedLeftChanged(objectiveCompletedBoxes);
        }

        if (ScoreChanged != null)
        {
            ScoreChanged(totalScore);
        }

        if (LifesLeftChanged != null)
        {
            LifesLeftChanged(livesLeft);
        }
    }

    private void OnBeerBoxCompleted(int newBoxesCompleted)
    {
        int boxesCompleted = boxesCompletedToMultiplier[Mathf.Clamp(newBoxesCompleted, 1, boxesCompletedToMultiplier.Length) - 1];
        completedBoxes += boxesCompleted;
        totalScore += boxesCompleted * scorePerBoxCompleted;

        if (BoxesCompletedLeftChanged != null)
        {
            BoxesCompletedLeftChanged(Mathf.Max(0, objectiveCompletedBoxes - completedBoxes));
        }

        if (ScoreChanged != null)
        {
            ScoreChanged(totalScore);
        }

        if (completedBoxes >= objectiveCompletedBoxes && levelNumber < 9)
        {
            levelNumber++;
            OnLevelChanged();
        }

    }

    private void OnBeerBoxRuined()
    {
        livesLeft = Mathf.Max(0, livesLeft - 1);

        if (LifesLeftChanged != null)
        {
            LifesLeftChanged(livesLeft);
        }

        if (livesLeft <= 0 && GameOver != null)
        {
            GameOver(totalScore);
        }
    }
}

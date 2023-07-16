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

    [Header("Parameters")]
    [SerializeField]
    [Range(0, 9)]
    private int levelNumber = 0;
    [SerializeField]
    private int[] levelToObjectiveBoxesCount = new int[10];
    [SerializeField]
    private int[] boxesCompletedToMultiplier;
    private float difficultyIncrease = 1 / 9f;



    private float difficulty;
    int completedBoxes = 0;
    int objectiveCompletedBoxes = 0;

    void Start()
    {
        OnLevelChanged();
    }

    void OnEnable()
    {
        Level.BeerBoxCompleted += OnBeerBoxCompleted;
    }

    void OnDisable()
    {
        Level.BeerBoxCompleted -= OnBeerBoxCompleted;
    }

    private void OnLevelChanged()
    {
        completedBoxes = 0;
        objectiveCompletedBoxes = levelToObjectiveBoxesCount[levelNumber];
        difficulty = difficultyIncrease * levelNumber;
        if (DifficultyChanged != null)
        {
            DifficultyChanged(difficulty, levelNumber + 1);
        }

        if (BoxesCompletedLeftChanged != null)
        {
            BoxesCompletedLeftChanged(objectiveCompletedBoxes);
        }
    }

    private void OnBeerBoxCompleted(int boxesCompleted)
    {
        completedBoxes += boxesCompletedToMultiplier[Mathf.Clamp(boxesCompleted, 1, boxesCompletedToMultiplier.Length) - 1];

        if (BoxesCompletedLeftChanged != null)
        {
            BoxesCompletedLeftChanged(Mathf.Max(0, objectiveCompletedBoxes - completedBoxes));
        }

        if (completedBoxes >= objectiveCompletedBoxes && levelNumber < 9)
        {
            levelNumber++;
            OnLevelChanged();
        }

    }
}

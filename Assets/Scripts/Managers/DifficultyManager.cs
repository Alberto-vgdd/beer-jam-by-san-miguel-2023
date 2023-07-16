using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public delegate void DifficultyChangedHandler(float newDifficulty);
    public static DifficultyChangedHandler DifficultyChanged;

    [Header("Parameters")]
    [SerializeField]
    [Range(0f, 1f)]
    private float difficulty;
    [SerializeField]
    private int completedBoxesToIncreaseDifficulty = 10;
    [SerializeField]
    private float difficultyIncrease = 0.1f;
    [SerializeField]
    private int[] boxesCompletedToScore;


    int currentCompletedBoxes = 0;
    void Start()
    {
        OnDifficultyChanged();
    }

    void OnEnable()
    {
        Level.BeerBoxCompleted += OnBeerBoxCompleted;
    }

    void OnDisable()
    {
        Level.BeerBoxCompleted -= OnBeerBoxCompleted;
    }

    private void OnDifficultyChanged()
    {
        if (DifficultyChanged != null)
        {
            DifficultyChanged(difficulty);
        }
    }

    private void OnBeerBoxCompleted(int boxesCompleted)
    {
        currentCompletedBoxes += boxesCompletedToScore[Mathf.Clamp(boxesCompleted, 1, boxesCompletedToScore.Length) - 1];
        if (currentCompletedBoxes >= completedBoxesToIncreaseDifficulty)
        {
            currentCompletedBoxes = 0;
            difficulty = Mathf.Clamp01(difficulty + difficultyIncrease);
            OnDifficultyChanged();
        }

    }
}

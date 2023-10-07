using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProgressionDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private TMP_Text levelNumberText;
    [SerializeField]
    private TMP_Text boxesLeftText;
    [SerializeField]
    private TMP_Text scoreText;


    int previousBoxesLeft = 0;
    int previousScore = 0;
    private Tween levelNumberAnimation;
    private Tween boxesLeftAnimation;
    private Tween scoreAnimation;

    void OnEnable()
    {
        // DifficultyManager.BoxesCompletedLeftChanged += OnBoxesCompletedLeftChanged;
        // DifficultyManager.PlayerDifficultyChanged += OnDifficulty;
        // DifficultyManager.ScoreChanged += OnScoreChanged;
    }

    void OnDisable()
    {
        // DifficultyManager.BoxesCompletedLeftChanged -= OnBoxesCompletedLeftChanged;
        // DifficultyManager.PlayerDifficultyChanged -= OnDifficulty;
        // DifficultyManager.ScoreChanged -= OnScoreChanged;

    }



    private void OnDifficulty(float newDifficulty, int newDisplayLevelNumber)
    {
        levelNumberText.text = newDisplayLevelNumber.ToString();
        if (newDisplayLevelNumber > 1)
        {
            DOTweenUtils.CompleteTween(levelNumberAnimation);
            levelNumberAnimation = levelNumberText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 4, 0.3f);
        }
    }

    private void OnBoxesCompletedLeftChanged(int newBoxesCompletedLeft)
    {
        boxesLeftText.text = newBoxesCompletedLeft.ToString();
        if (newBoxesCompletedLeft < previousBoxesLeft)
        {
            DOTweenUtils.CompleteTween(boxesLeftAnimation);
            boxesLeftAnimation = boxesLeftText.transform.DOPunchScale(-Vector3.one * 0.3f, 0.2f, 4, 0.3f);
        }
        previousBoxesLeft = newBoxesCompletedLeft;
    }

    private void OnScoreChanged(int newScore)
    {
        scoreText.text = newScore.ToString();
        if (newScore > 0 && previousScore != newScore)
        {
            DOTweenUtils.CompleteTween(scoreAnimation);
            scoreAnimation = scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6, 0.3f);
        }
        previousScore = newScore;
    }

}
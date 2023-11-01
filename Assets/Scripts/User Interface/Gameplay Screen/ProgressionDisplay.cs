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

    [Header("Parameters")]
    [SerializeField]
    private int playerNumber;
    [SerializeField]
    private int textFontWeight = 700;


    void OnEnable()
    {
        DifficultyManager.PlayerBoxesCompletedLeftChanged[playerNumber] += OnBoxesCompletedLeftChanged;
        DifficultyManager.PlayerDifficultyChanged[playerNumber] += OnDifficulty;
        DifficultyManager.PlayerScoreChanged[playerNumber] += OnScoreChanged;
    }

    void OnDisable()
    {
        DifficultyManager.PlayerBoxesCompletedLeftChanged[playerNumber] -= OnBoxesCompletedLeftChanged;
        DifficultyManager.PlayerDifficultyChanged[playerNumber] -= OnDifficulty;
        DifficultyManager.PlayerScoreChanged[playerNumber] -= OnScoreChanged;
    }



    private void OnDifficulty(float newDifficulty, int newDisplayLevelNumber)
    {
        levelNumberText.text = StringUtils.FormatStringWithFontWeight(newDisplayLevelNumber.ToString(), textFontWeight);
        if (newDisplayLevelNumber > 1)
        {
            DOTweenUtils.CompleteTween(levelNumberAnimation);
            levelNumberAnimation = levelNumberText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 4, 0.3f);
        }
    }

    private void OnBoxesCompletedLeftChanged(int newBoxesCompletedLeft)
    {
        boxesLeftText.text = StringUtils.FormatStringWithFontWeight(newBoxesCompletedLeft.ToString(), textFontWeight);
        if (newBoxesCompletedLeft < previousBoxesLeft)
        {
            DOTweenUtils.CompleteTween(boxesLeftAnimation);
            boxesLeftAnimation = boxesLeftText.transform.DOPunchScale(-Vector3.one * 0.3f, 0.2f, 4, 0.3f);
        }
        previousBoxesLeft = newBoxesCompletedLeft;
    }

    private void OnScoreChanged(int newScore)
    {
        scoreText.text = StringUtils.FormatStringWithFontWeight(newScore.ToString(), textFontWeight);
        if (newScore > 0 && previousScore != newScore)
        {
            DOTweenUtils.CompleteTween(scoreAnimation);
            scoreAnimation = scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6, 0.3f);
        }
        previousScore = newScore;
    }

}
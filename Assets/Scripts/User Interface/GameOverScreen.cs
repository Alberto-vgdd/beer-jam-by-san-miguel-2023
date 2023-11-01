using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private TMP_Text totalScoreText;
    [SerializeField]
    private GameObject playAgainButton;

    [SerializeField]
    private int scoreTextFontWeight = 900;

    internal void SetTotalScore(int newTotalScore)
    {
        totalScoreText.text = StringUtils.FormatStringWithFontWeight(newTotalScore.ToString(), scoreTextFontWeight);
    }

    public void OnGameOverAnimationFinished()
    {
        SelectGameObjectRequested?.Invoke(playAgainButton);
    }
}

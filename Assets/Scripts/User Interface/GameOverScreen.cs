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

    internal void SetTotalScore(int newTotalScore)
    {
        totalScoreText.text = newTotalScore.ToString();
    }


    public void OnGameOverAnimationFinished()
    {
        SelectGameObjectRequested?.Invoke(playAgainButton);
    }
}

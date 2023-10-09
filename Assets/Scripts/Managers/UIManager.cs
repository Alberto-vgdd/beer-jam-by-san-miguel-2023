using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject gameOverScreenGameObject;
    [SerializeField]
    private GameObject titleScreenGameObject;
    [SerializeField]
    private GameObject gameplayScreenGameObject;

    private GameOverScreen gameOverScreen;

    void Awake()
    {
        gameOverScreen = gameOverScreenGameObject.GetComponent<GameOverScreen>();
    }


    public void ShowGameOverScreen(int winnerPlayerNumber, PlayerProgress[] playersGameProgresses)
    {
        gameOverScreenGameObject.SetActive(true);
        titleScreenGameObject.SetActive(false);
        gameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore);
    }

    internal void ShowGameplayScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(false);
        gameplayScreenGameObject.SetActive(true);
    }

    internal void ShowTitleScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(true);
        gameplayScreenGameObject.SetActive(false);
    }
}

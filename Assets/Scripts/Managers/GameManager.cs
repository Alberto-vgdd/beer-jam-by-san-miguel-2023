using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField]
    private UIManager uIManager;


    void OnEnable()
    {
        DifficultyManager.GameOver += OnGameOver;
    }

    private void OnGameOver(int newScore)
    {
        DifficultyManager.GameOver -= OnGameOver;
        uIManager.ShowGameOverScreen(newScore);
    }

    public void StartNewGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }




    protected override GameManager GetThis()
    {
        return this;
    }

}

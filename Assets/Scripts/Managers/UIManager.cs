using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject gameOverScreenGameObject;
    [SerializeField]
    private GameObject titleScreenGameObject;
    [SerializeField]
    private GameObject soloGameplayScreenGameObject;
    [SerializeField]
    private GameObject multiplayerGameplayScreenGameObject;
    [SerializeField]
    private EventSystem eventSystem;

    private GameOverScreen gameOverScreen;

    [SerializeField]
    private GameFinishedDisplay gameFinishedDisplay;

    void Awake()
    {
        gameOverScreen = gameOverScreenGameObject.GetComponent<GameOverScreen>();
    }

    void OnEnable()
    {
        BaseScreen.SelectGameObjectRequested += OnSelectGameObjectRequested;
    }


    void OnDisable()
    {
        BaseScreen.SelectGameObjectRequested -= OnSelectGameObjectRequested;
    }

    public void ShowGameOverScreen(int winnerPlayerNumber, PlayerProgress[] playersGameProgresses)
    {
        gameOverScreenGameObject.SetActive(true);

        if (InputManager.NUMBER_OF_PLAYERS > 1)
        {
            gameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore, winnerPlayerNumber, true);
        }
        else
        {
            gameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore, winnerPlayerNumber, false);
        }

        titleScreenGameObject.SetActive(false);
    }

    internal void ShowGameplayScreen()
    {
        gameOverScreenGameObject.SetActive(false);

        titleScreenGameObject.SetActive(false);

        soloGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 1);
        multiplayerGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 2);

        if (InputManager.NUMBER_OF_PLAYERS == 2)
        {
            gameFinishedDisplay.Reset();
        }

    }

    internal void ShowTitleScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(true);
        soloGameplayScreenGameObject.SetActive(false);
        multiplayerGameplayScreenGameObject.SetActive(false);
    }

    private void OnSelectGameObjectRequested(GameObject newGameObjectToSelect)
    {
        eventSystem.SetSelectedGameObject(newGameObjectToSelect);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject soloGameOverScreenGameObject;
    [SerializeField]
    private GameObject multiGameOverScreenGameObject;
    [SerializeField]
    private GameObject titleScreenGameObject;
    [SerializeField]
    private GameObject soloGameplayScreenGameObject;
    [SerializeField]
    private GameObject multiplayerGameplayScreenGameObject;

    private GameOverScreen soloGameOverScreen;
    private GameOverScreen multiGameOverScreen;

    void Awake()
    {
        soloGameOverScreen = soloGameOverScreenGameObject.GetComponent<GameOverScreen>();
        multiGameOverScreen = multiGameOverScreenGameObject.GetComponent<GameOverScreen>();
    }


    public void ShowGameOverScreen(int winnerPlayerNumber, PlayerProgress[] playersGameProgresses)
    {
        if (InputManager.NUMBER_OF_PLAYERS > 1)
        {
            soloGameOverScreenGameObject.SetActive(false);
            multiGameOverScreenGameObject.SetActive(true);
            multiGameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore);
        }
        else
        {
            soloGameOverScreenGameObject.SetActive(true);
            multiGameOverScreenGameObject.SetActive(false);
            soloGameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore);
        }

        titleScreenGameObject.SetActive(false);


    }

    internal void ShowGameplayScreen()
    {
        soloGameOverScreenGameObject.SetActive(false);
        multiGameOverScreenGameObject.SetActive(false);

        titleScreenGameObject.SetActive(false);

        soloGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 1);
        multiplayerGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 2);
    }

    internal void ShowTitleScreen()
    {
        soloGameOverScreenGameObject.SetActive(false);
        multiGameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(true);
        soloGameplayScreenGameObject.SetActive(false);
        multiplayerGameplayScreenGameObject.SetActive(false);
    }
}

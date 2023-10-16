using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishedDisplay : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject[] gameFinishedDisplays;
    [SerializeField]
    private GameObject[] waitingForOtherPlayersMessages;

    private int gameFinishedCount = 0;


    private void Awake()
    {
        foreach (GameObject gameFinishedDisplay in gameFinishedDisplays)
        {
            gameFinishedDisplay.SetActive(false);
        }
    }
    private void OnGameOver(PlayerProgress playerProgress)
    {
        gameFinishedDisplays[playerProgress.playerNumber].SetActive(true);
        waitingForOtherPlayersMessages[playerProgress.playerNumber].SetActive(true);
        gameFinishedCount++;

        if (gameFinishedCount >= InputManager.NUMBER_OF_PLAYERS)
        {
            for (int playerNumber = 0; playerNumber < InputManager.NUMBER_OF_PLAYERS; playerNumber++)
            {
                DifficultyManager.GameOver[playerNumber] -= OnGameOver;
                waitingForOtherPlayersMessages[playerNumber].SetActive(false);
            }
        }
    }

    internal void Reset()
    {
        foreach (GameObject gameFinishedDisplay in gameFinishedDisplays)
        {
            gameFinishedDisplay.SetActive(false);
        }
        gameFinishedCount = 0;

        for (int playerNumber = 0; playerNumber < InputManager.NUMBER_OF_PLAYERS; playerNumber++)
        {
            gameFinishedDisplays[playerNumber].SetActive(false);
            DifficultyManager.GameOver[playerNumber] += OnGameOver;
        }
    }
}

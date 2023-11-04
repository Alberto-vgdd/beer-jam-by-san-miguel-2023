using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : BaseScreen
{
    private const string SINGLE_PLAYER_GAME_OVER_MESSAGE = "FIN DE LA PARTIDA";
    private const string MULTIPLAYER_PLAYER_GAME_OVER_MESSAGE = "HA GANADO EL JUGADOR {0}";

    [Header("Components")]
    [SerializeField]
    private TMP_Text totalScoreText;
    [SerializeField]
    private TMP_Text gameOverMessageText;
    [SerializeField]
    private GameObject playAgainButton;

    [SerializeField]
    private int scoreTextFontWeight = 900;
    [SerializeField]
    private int gameOverMessageFontWeight = 900;

    internal void SetTotalScore(int newTotalScore, int winnerPlayerNumber, bool isMultiplayer)
    {
        totalScoreText.text = StringUtils.FormatStringWithFontWeight(newTotalScore.ToString(), scoreTextFontWeight);

        if (isMultiplayer)
        {
            gameOverMessageText.text = StringUtils.FormatStringWithFontWeight(String.Format(MULTIPLAYER_PLAYER_GAME_OVER_MESSAGE, winnerPlayerNumber + 1), gameOverMessageFontWeight);
        }
        else
        {
            gameOverMessageText.text = StringUtils.FormatStringWithFontWeight(SINGLE_PLAYER_GAME_OVER_MESSAGE, gameOverMessageFontWeight);

        }

    }

    public void OnGameOverAnimationFinished()
    {
        SelectGameObjectRequested?.Invoke(playAgainButton);
    }
}

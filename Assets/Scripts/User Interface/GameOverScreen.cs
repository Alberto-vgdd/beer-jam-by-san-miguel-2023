using System;
using TMPro;
using UnityEngine;

public class GameOverScreen : BaseScreen
{
    private const string SINGLE_PLAYER_GAME_OVER_MESSAGE = "FIN DE LA PARTIDA";
    private const string MULTIPLAYER_PLAYER_GAME_OVER_MESSAGE = "HA GANADO EL JUGADOR {0}";

    private const string IS_NEW_RECORD_PARAMETER = "isNewRecord";

    [Header("Components")]
    [SerializeField]
    private TMP_Text totalScoreText;
    [SerializeField]
    private TMP_Text gameOverMessageText;
    [SerializeField]
    private GameObject playAgainButton;
    [SerializeField]
    private GameObject toEnterNewNameScreenButton;
    [SerializeField]
    private Animator gameOverScreenAnimator;

    [SerializeField]
    private int scoreTextFontWeight = 900;
    [SerializeField]
    private int gameOverMessageFontWeight = 900;

    private bool isNewRecord;
    internal void SetTotalScore(int newTotalScore, int winnerPlayerNumber, bool isMultiplayer, bool newIsNewRecord)
    {
        isNewRecord = newIsNewRecord;
        gameOverScreenAnimator.SetBool(IS_NEW_RECORD_PARAMETER, isNewRecord);

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
        if (isNewRecord)
        {
            SelectGameObjectRequested?.Invoke(toEnterNewNameScreenButton);
        }
        else
        {
            SelectGameObjectRequested?.Invoke(playAgainButton);

        }
    }
}

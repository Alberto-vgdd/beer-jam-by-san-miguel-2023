using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scorePositionText;
    [SerializeField]
    private TextMeshProUGUI playerNameText;
    [SerializeField]
    private TextMeshProUGUI moneyAmountText;


    [SerializeField]
    private int scorePositionTextFontWeight = 900;
    [SerializeField]
    private int playerNameTextFontWeight = 300;
    [SerializeField]
    private int moneyAmountNameTextFontWeight = 900;


    private int scorePosition = -1;
    private int moneyAmount = 0;
    private string playerName = "NLL";

    internal void SetScorePosition(int newPosition)
    {
        scorePosition = newPosition;
        scorePositionText.text = StringUtils.FormatStringWithFontWeight(newPosition + ".", scorePositionTextFontWeight);
    }

    internal void SetPlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        playerNameText.text = StringUtils.FormatStringWithFontWeight(newPlayerName, playerNameTextFontWeight);
    }

    internal void SetMoneyAmount(int newMoneyAmount)
    {
        moneyAmount = newMoneyAmount;
        moneyAmountText.text = StringUtils.FormatStringWithFontWeight(newMoneyAmount + " $", moneyAmountNameTextFontWeight);
    }


    internal int GetMoneyAmount()
    {
        return moneyAmount;
    }

    internal int GetScorePosition()
    {
        return scorePosition;
    }

    internal string GetPlayerName()
    {
        return playerName;
    }
}

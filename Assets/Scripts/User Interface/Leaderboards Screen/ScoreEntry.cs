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


    private int scorePosition = -1;
    private int moneyAmount = 0;
    private string playerName = "NLL";

    internal void SetScorePosition(int newPosition)
    {
        scorePosition = newPosition;
        scorePositionText.text = newPosition + ".";
    }

    internal void SetPlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        playerNameText.text = newPlayerName;
    }

    internal void SetMoneyAmount(int newMoneyAmount)
    {
        moneyAmount = newMoneyAmount;
        moneyAmountText.text = newMoneyAmount + " $";
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

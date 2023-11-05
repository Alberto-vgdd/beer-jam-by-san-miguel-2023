using System;
using UnityEngine;

public class LeaderboardsManager : Singleton<LeaderboardsManager>
{
    private const int CURRENT_LEADERBOARDS_VERSION = 3;
    private const string SCORE = "score";
    private const string NAME = "name";
    private const string VERSION = "version";
    private const int NUMBER_OF_SCORES = 5;

    private class NewScore
    {
        internal int scorePosition = -1;
        internal int moneyAmount = -1;
        internal string playerName = "";

        internal void Reset()
        {
            scorePosition = -1;
            moneyAmount = -1;
            playerName = "";
        }

        internal bool IsValid()
        {
            return scorePosition > -1 && scorePosition < NUMBER_OF_SCORES && moneyAmount > -1 && playerName.Length > 0;
        }
    }


    [SerializeField]
    private LeaderboardsScreen LeaderboardsScreen;
    [SerializeField]
    private ScoreEntry scoreEntryPrefab;
    [SerializeField]
    private ScoreEntry[] scores;

    private NewScore newScore = new NewScore();


    void Start()
    {
        InitialiseLeaderboards();
        LoadLeaderboardsData();
    }


    private void InitialiseLeaderboards()
    {
        scores = new ScoreEntry[NUMBER_OF_SCORES];
        ScoreEntry scoreEntry;
        for (int i = 0; i < NUMBER_OF_SCORES; i++)
        {
            scoreEntry = Instantiate<ScoreEntry>(scoreEntryPrefab, LeaderboardsScreen.GetScoreEntryHolder());
            scoreEntry.SetScorePosition(i + 1);
            scores[i] = scoreEntry;
        }
    }

    private void LoadLeaderboardsData()
    {
        SetScore(0, "ALBER", 600);
        SetScore(1, "LAURA", 500);
        SetScore(2, "JAUME", 425);
        SetScore(3, "NADIA", 375);
        SetScore(4, "JONA", 300);


        if (PlayerPrefs.GetInt(VERSION, 0) == CURRENT_LEADERBOARDS_VERSION)
        {
            for (int i = 0; i < NUMBER_OF_SCORES; i++)
            {
                if (PlayerPrefs.HasKey(SCORE + i))
                {
                    SetScore(i, PlayerPrefs.GetString(NAME + i, "ERROR"), PlayerPrefs.GetInt(SCORE + i, -1));
                }
            }
        }
        else
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt(VERSION, CURRENT_LEADERBOARDS_VERSION);
        }
    }

    internal int CheckForNewScore(int amount)
    {
        newScore.Reset();
        int newRecord = -1;

        for (int i = 0; i < NUMBER_OF_SCORES; i++)
        {
            if (amount > scores[i].GetMoneyAmount())
            {
                newRecord = i;
                newScore.scorePosition = i;
                newScore.moneyAmount = amount;
                break;
            }
        }

        return newRecord;

    }

    internal void SetNewRecordPlayerName(string newPlayerName)
    {
        newScore.playerName = newPlayerName;
        if (newScore.IsValid())
        {
            SetNewScore(newScore);
        }
        else
        {
            throw new UnityException("Invalid score. position: " + newScore.scorePosition + " name: " + newScore.playerName + " amount: " + newScore.moneyAmount);
        }
    }

    private void SetNewScore(NewScore score)
    {
        int position = score.scorePosition;

        string newPlayerName = score.playerName;
        int newMoneyAmount = score.moneyAmount;

        string movedPlayerName;
        int movedMoneyAmount;

        for (int i = position; i < NUMBER_OF_SCORES; i++)
        {
            movedPlayerName = scores[i].GetPlayerName();
            movedMoneyAmount = scores[i].GetMoneyAmount();

            SetScore(i, newPlayerName, newMoneyAmount);
            SaveScoreEntry(i, newPlayerName, newMoneyAmount);

            newPlayerName = movedPlayerName;
            newMoneyAmount = movedMoneyAmount;
        }

        score.Reset();
    }

    private static void SaveScoreEntry(int position, string newPlayerName, int newMoneyAmount)
    {
        PlayerPrefs.SetInt(SCORE + position, newMoneyAmount);
        PlayerPrefs.SetString(NAME + position, newPlayerName);
        PlayerPrefs.Save();
    }

    private void SetScore(int position, string newPlayerName, int newMoneyAmount)
    {
        scores[position].SetPlayerName(newPlayerName);
        scores[position].SetMoneyAmount(newMoneyAmount);
    }

    protected override LeaderboardsManager GetThis()
    {
        return this;
    }
}


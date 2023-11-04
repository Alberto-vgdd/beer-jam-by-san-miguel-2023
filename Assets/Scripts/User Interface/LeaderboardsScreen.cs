using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject backToMenuButton;

    [SerializeField]
    private Transform scoresHolder;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(backToMenuButton);
    }

    internal Transform GetScoreEntryHolder()
    {
        return scoresHolder;
    }



}

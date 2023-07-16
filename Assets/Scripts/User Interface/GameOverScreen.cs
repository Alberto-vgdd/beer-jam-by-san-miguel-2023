using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private TMP_Text totalScoreText;

    internal void SetTotalScore(int newTotalScore)
    {
        totalScoreText.text = newTotalScore.ToString();
    }
}

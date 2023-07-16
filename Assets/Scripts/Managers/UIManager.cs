using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private GameObject gameOverScreenGameObject;


    private GameOverScreen gameOverScreen;

    void Awake()
    {
        gameOverScreen = gameOverScreenGameObject.GetComponent<GameOverScreen>();
    }


    public void ShowGameOverScreen(int totalScore)
    {
        gameOverScreenGameObject.SetActive(true);
        gameOverScreen.SetTotalScore(totalScore);
    }
}

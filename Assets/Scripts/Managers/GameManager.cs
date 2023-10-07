using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private DifficultyManager difficultyManager;

    private PlayerControls playerControls;

    [SerializeField]
    private PlayerTable[] playerTables;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    void Start()
    {
        uIManager.ShowTitleScreen();
    }

    void OnEnable()
    {
        playerControls.Navigation.ExitGame.performed += OnExitGameButtonPressed;
    }

    void OnDisable()
    {
        playerControls.Navigation.ExitGame.performed -= OnExitGameButtonPressed;
    }

    // TODO FIX GAMEOVER STUFF
    private void OnGameOver(int playerNumber, int newScore)
    {
        InputManager.Instance.PauseInputs(true);
        DifficultyManager.GameOver -= OnGameOver;
        uIManager.ShowGameOverScreen(newScore);
        foreach (PlayerTable playerTable in playerTables)
        {
            playerTable.StopGame();
        }

    }

    public void StartNewGame()
    {
        uIManager.ShowGameplayScreen();
        foreach (PlayerTable playerTable in playerTables)
        {
            playerTable.StartGame();
        }
        difficultyManager.ResetProgress();
        InputManager.Instance.PauseInputs(false);

        DifficultyManager.GameOver += OnGameOver;
    }

    private void OnExitGameButtonPressed(InputAction.CallbackContext context)
    {
        ExitGame();
    }


    public void ExitGame()
    {
        Application.Quit();
    }




    protected override GameManager GetThis()
    {
        return this;
    }

}

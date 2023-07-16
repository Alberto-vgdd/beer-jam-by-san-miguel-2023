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
    private Level level;

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

    private void OnGameOver(int newScore)
    {
        InputManager.PauseGameplayInputs(true);
        DifficultyManager.GameOver -= OnGameOver;
        uIManager.ShowGameOverScreen(newScore);
        level.StopGame();


    }

    public void StartNewGame()
    {
        uIManager.ShowGameplayScreen();
        level.StartGame();
        difficultyManager.ResetProgress();
        InputManager.PauseGameplayInputs(false);

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

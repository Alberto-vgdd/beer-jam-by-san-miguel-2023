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


    private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        playerControls.Enable();

    }

    void OnEnable()
    {
        DifficultyManager.GameOver += OnGameOver;
        playerControls.Navigation.ExitGame.performed += OnExitGameButtonPressed;
    }

    void OnDisable()
    {
        playerControls.Navigation.ExitGame.performed -= OnExitGameButtonPressed;
    }

    private void OnGameOver(int newScore)
    {
        DifficultyManager.GameOver -= OnGameOver;
        uIManager.ShowGameOverScreen(newScore);
    }

    public void StartNewGame()
    {

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

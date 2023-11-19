using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Components")]
    [SerializeField]
    private GameObject leaderBoardsScreenGameObject;
    [SerializeField]
    private GameObject enterNewNameScreenGameObject;
    [SerializeField]
    private GameObject gameOverScreenGameObject;
    [SerializeField]
    private GameObject titleScreenGameObject;
    [SerializeField]
    private GameObject soloGameplayScreenGameObject;
    [SerializeField]
    private GameObject multiplayerGameplayScreenGameObject;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private InputSystemUIInputModule inputSystemUIInputModule;

    [SerializeField]
    private GameFinishedDisplay gameFinishedDisplay;



    private GameOverScreen gameOverScreen;
    private EnterNewNameScreen enterNewNameScreen;
    private LeaderboardsScreen leaderboardsScreen;

    protected override void Awake()
    {
        base.Awake();
        gameOverScreen = gameOverScreenGameObject.GetComponent<GameOverScreen>();
    }

    void OnEnable()
    {
        BaseScreen.SelectGameObjectRequested += OnSelectGameObjectRequested;
    }


    void OnDisable()
    {
        BaseScreen.SelectGameObjectRequested -= OnSelectGameObjectRequested;
    }

    public void ShowGameOverScreen(int winnerPlayerNumber, PlayerProgress[] playersGameProgresses, bool isNewRecord)
    {
        gameOverScreenGameObject.SetActive(true);

        gameOverScreen.SetTotalScore(playersGameProgresses[winnerPlayerNumber].totalScore, winnerPlayerNumber, InputManager.NUMBER_OF_PLAYERS > 1, isNewRecord);

        titleScreenGameObject.SetActive(false);
        enterNewNameScreenGameObject.SetActive(false);
        leaderBoardsScreenGameObject.SetActive(false);
    }

    internal void ShowGameplayScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        enterNewNameScreenGameObject.SetActive(false);
        leaderBoardsScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(false);

        soloGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 1);
        multiplayerGameplayScreenGameObject.SetActive(InputManager.NUMBER_OF_PLAYERS == 2);

        if (InputManager.NUMBER_OF_PLAYERS == 2)
        {
            gameFinishedDisplay.Reset();
        }

    }

    internal void ShowTitleScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(true);
        soloGameplayScreenGameObject.SetActive(false);
        multiplayerGameplayScreenGameObject.SetActive(false);
        enterNewNameScreenGameObject.SetActive(false);
        leaderBoardsScreenGameObject.SetActive(false);
    }

    private void OnSelectGameObjectRequested(GameObject newGameObjectToSelect)
    {
        eventSystem.SetSelectedGameObject(newGameObjectToSelect);
    }

    protected override UIManager GetThis()
    {
        return this;
    }

    internal void ShowLeaderboardsScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(false);
        soloGameplayScreenGameObject.SetActive(false);
        multiplayerGameplayScreenGameObject.SetActive(false);
        enterNewNameScreenGameObject.SetActive(false);
        leaderBoardsScreenGameObject.SetActive(true);
    }

    internal void ShowEnterNewNameScreen()
    {
        gameOverScreenGameObject.SetActive(false);
        titleScreenGameObject.SetActive(false);
        soloGameplayScreenGameObject.SetActive(false);
        multiplayerGameplayScreenGameObject.SetActive(false);
        enterNewNameScreenGameObject.SetActive(true);
        leaderBoardsScreenGameObject.SetActive(false);
    }

    internal void OnlyReadInputsFrom(PlayerControls playerControls)
    {
        inputSystemUIInputModule.actionsAsset.devices = playerControls.devices;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : BaseScreen
{
    private const string HIDE_POP_UP_STRING = "Hide Pop Up";


    [Header("Components")]
    [SerializeField]
    private GameObject continueButton;
    [SerializeField]
    private Animator popUpAnimator;

    [SerializeField]
    private GameObject titleScreenGameObject;


    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(continueButton);
    }


    public void ContinueButtonPressed()
    {
        popUpAnimator.SetTrigger(HIDE_POP_UP_STRING);
    }

    public void OnTutorialBackgroundOpaque()
    {
        titleScreenGameObject.SetActive(false);
    }

    public void OnHidePopUpAnimationFinished()
    {
        this.gameObject.SetActive(false);
    }

    public void OnStartNewGame()
    {
        GameManager.Instance.StartNewGame(1);
    }
}

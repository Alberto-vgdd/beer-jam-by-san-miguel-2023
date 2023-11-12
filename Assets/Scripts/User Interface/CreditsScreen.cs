using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen : BaseScreen
{
    private const string HIDE_POP_UP_STRING = "Hide Pop Up";

    [Header("Components")]
    [SerializeField]
    private GameObject backToMenuButton;
    [SerializeField]
    private Animator popUpAnimator;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(backToMenuButton);
    }

    public void ClosePopUpButtonPressed()
    {
        popUpAnimator.SetTrigger(HIDE_POP_UP_STRING);
    }

    public void OnHidePopUpAnimationFinished()
    {
        this.gameObject.SetActive(false);
    }
}

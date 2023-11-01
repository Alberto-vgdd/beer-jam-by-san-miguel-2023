using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject continueButton;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(continueButton);
    }
}

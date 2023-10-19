using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject backToMenuButton;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(backToMenuButton);
    }
}

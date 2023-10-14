using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject startButton;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(startButton);
    }
}

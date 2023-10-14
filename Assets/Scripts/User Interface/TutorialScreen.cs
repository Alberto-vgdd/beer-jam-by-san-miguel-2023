using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject onePlayerGame;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(onePlayerGame);
    }
}

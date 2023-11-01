using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSelectionScreen : BaseScreen
{
    [Header("Components")]
    [SerializeField]
    private GameObject onePlayerGameButton;

    void OnEnable()
    {
        SelectGameObjectRequested?.Invoke(onePlayerGameButton);
    }
}

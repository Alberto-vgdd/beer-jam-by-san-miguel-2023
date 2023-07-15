using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void InputEnabledHandler(bool newEnabled);
    public static InputEnabledHandler InputEnabled;

    private delegate void GameplayInputsPausedHandler(bool newGameplayInputsPaused);
    private static GameplayInputsPausedHandler GameplayInputsPaused;


    private bool inputsPaused = false;


    public static void PauseGameplayInputs(bool newGameplayInputsPaused)
    {
        if (GameplayInputsPaused != null)
        {
            GameplayInputsPaused(newGameplayInputsPaused);
        }
    }


    void Awake()
    {
        InputManager.GameplayInputsPaused += PauseInputs;
    }


    private void PauseInputs(bool newInputsPaused)
    {
        inputsPaused = newInputsPaused;

        if (InputEnabled != null)
        {
            InputEnabled(!inputsPaused);
        }
    }
}

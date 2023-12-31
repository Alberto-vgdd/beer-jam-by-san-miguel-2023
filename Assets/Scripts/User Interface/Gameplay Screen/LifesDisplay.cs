using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private LifeContainer[] lifes;

    [Header("Parameters")]
    [SerializeField]
    private int playerNumber;

    void OnEnable()
    {
        DifficultyManager.PlayerLifesLeftChanged[playerNumber] += OnLifesLeftChanged;

    }

    void OnDisable()
    {
        DifficultyManager.PlayerLifesLeftChanged[playerNumber] -= OnLifesLeftChanged;

    }

    private void OnLifesLeftChanged(int numberOfLivesLeft)
    {
        for (int i = 0; i < lifes.Length; i++)
        {
            if (i < numberOfLivesLeft)
            {
                lifes[i].RecoverLife();
            }
            else
            {
                lifes[i].UseLife();

            }
        }
    }
}

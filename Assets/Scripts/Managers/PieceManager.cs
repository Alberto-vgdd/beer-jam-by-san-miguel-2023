using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singleton<PieceManager>
{

    [Header("Components")]
    [SerializeField]
    private BottlePiece[] bottlePieces;
    [SerializeField]
    private AnimationCurve easyPieceChancesCurve;
    [SerializeField]
    private AnimationCurve difficultPieceChancesCurve;


    private float difficulty;


    void OnEnable()
    {
        DifficultyManager.DifficultyChanged += OnDifficultyChanged;
    }
    void OnDisable()
    {
        DifficultyManager.DifficultyChanged -= OnDifficultyChanged;
    }

    private void OnDifficultyChanged(float newDifficulty)
    {
        difficulty = newDifficulty;
    }

    internal BottlePiece GetRandomBottlePiece()
    {

        float randomValue = Random.value;
        float floatIndex = Mathf.Lerp(easyPieceChancesCurve.Evaluate(randomValue), difficultPieceChancesCurve.Evaluate(randomValue), difficulty);
        return bottlePieces[Mathf.RoundToInt((bottlePieces.Length - 1) * floatIndex)];
    }


    protected override PieceManager GetThis()
    {
        return this;
    }
}

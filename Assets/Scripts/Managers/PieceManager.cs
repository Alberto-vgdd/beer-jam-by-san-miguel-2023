using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singleton<PieceManager>
{
    public delegate void NextPieceChangedHandler(BottlePiece bottlePiece);
    public static NextPieceChangedHandler NextPieceChanged;

    [Header("Components")]
    [SerializeField]
    private BottlePiece[] bottlePieces;
    [SerializeField]
    private BottleVisuals[] bottleVisuals;
    [SerializeField]
    private AnimationCurve easyPieceChancesCurve;
    [SerializeField]
    private AnimationCurve difficultPieceChancesCurve;

    private float difficulty;

    private BottlePiece nextPiece;


    void OnEnable()
    {
        DifficultyManager.DifficultyChanged += OnDifficultyChanged;
    }
    void OnDisable()
    {
        DifficultyManager.DifficultyChanged -= OnDifficultyChanged;
    }

    private void OnDifficultyChanged(float newDifficulty, int newLevelDisplayNumber)
    {
        difficulty = newDifficulty;
    }

    void Start()
    {
        UpdateNextPiece();
    }

    internal BottlePiece GetRandomPiece()
    {
        BottlePiece randomPiece = nextPiece;
        randomPiece.gameObject.SetActive(true);

        UpdateNextPiece();

        return randomPiece;


    }

    internal void UpdateNextPiece()
    {
        float randomValue = Random.value;
        float floatIndex = Mathf.Lerp(easyPieceChancesCurve.Evaluate(randomValue), difficultPieceChancesCurve.Evaluate(randomValue), difficulty);
        nextPiece = Instantiate<BottlePiece>(bottlePieces[Mathf.RoundToInt((bottlePieces.Length - 1) * floatIndex)]);
        nextPiece.Initialise();
        int numberOfBottles = nextPiece.GetNumberOfBottles();
        nextPiece.SetBottlesVisuals(GetRandomBottles(numberOfBottles));

        if (NextPieceChanged != null)
        {
            NextPieceChanged(nextPiece);
        }

        nextPiece.gameObject.SetActive(false);
    }



    private BottleVisuals[] GetRandomBottles(int numberOfBottles)
    {
        BottleVisuals[] randomBottles = new BottleVisuals[numberOfBottles];

        for (int i = 0; i < numberOfBottles; i++)
        {
            randomBottles[i] = Instantiate<BottleVisuals>(bottleVisuals[Random.Range(0, bottleVisuals.Length)]);
        }

        return randomBottles;
    }

    protected override PieceManager GetThis()
    {
        return this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singleton<PieceManager>
{
    public delegate void PlayerNextPieceChangedHandler(BottlePiece bottlePiece);
    public static PlayerNextPieceChangedHandler[] PlayerNextPieceChanged = new PlayerNextPieceChangedHandler[InputManager.NUMBER_OF_PLAYERS];

    [Header("Components")]
    [SerializeField]
    private BottlePiece[] bottlePieces;
    [SerializeField]
    private BottleVisuals[] bottleVisuals;
    [SerializeField]
    private AnimationCurve easyPieceChancesCurve;
    [SerializeField]
    private AnimationCurve difficultPieceChancesCurve;

    private PlayerPieceDifficulty[] playersPieceDifficulties;

    protected override void Awake()
    {
        base.Awake();

        playersPieceDifficulties = new PlayerPieceDifficulty[InputManager.NUMBER_OF_PLAYERS];
        for (int playerNumber = 0; playerNumber < InputManager.NUMBER_OF_PLAYERS; playerNumber++)
        {
            playersPieceDifficulties[playerNumber] = new PlayerPieceDifficulty(playerNumber);
        }
    }


    void OnEnable()
    {
        foreach (PlayerPieceDifficulty playerPieceDifficulty in playersPieceDifficulties)
        {
            DifficultyManager.PlayerDifficultyChanged[playerPieceDifficulty.playerNumber] += playerPieceDifficulty.OnDifficultyChanged;
        }
    }
    void OnDisable()
    {
        foreach (PlayerPieceDifficulty playerPieceDifficulty in playersPieceDifficulties)
        {
            DifficultyManager.PlayerDifficultyChanged[playerPieceDifficulty.playerNumber] -= playerPieceDifficulty.OnDifficultyChanged;
        }
    }

    void Start()
    {
        foreach (PlayerPieceDifficulty playerPieceDifficulty in playersPieceDifficulties)
        {
            UpdateNextPiece(playerPieceDifficulty);
        }
    }

    internal BottlePiece GetRandomPiece(int playerNumber)
    {

        PlayerPieceDifficulty playerPieceDifficulty = playersPieceDifficulties[playerNumber];
        BottlePiece randomPiece = playerPieceDifficulty.nextPiece;
        randomPiece.gameObject.SetActive(true);
        UpdateNextPiece(playerPieceDifficulty);

        return randomPiece;
    }

    internal void UpdateNextPiece(PlayerPieceDifficulty playerPieceDifficulty)
    {

        float randomValue = Random.value;
        float floatIndex = Mathf.Lerp(easyPieceChancesCurve.Evaluate(randomValue), difficultPieceChancesCurve.Evaluate(randomValue), playerPieceDifficulty.difficulty);
        BottlePiece nextPiece = Instantiate<BottlePiece>(bottlePieces[Mathf.RoundToInt((bottlePieces.Length - 1) * floatIndex)]);
        nextPiece.Initialise();
        int numberOfBottles = nextPiece.GetNumberOfBottles();
        nextPiece.SetBottlesVisuals(GetRandomBottles(numberOfBottles));
        playerPieceDifficulty.nextPiece = nextPiece;

        PlayerNextPieceChanged[playerPieceDifficulty.playerNumber]?.Invoke(nextPiece);

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

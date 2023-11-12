using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerTable : MonoBehaviour
{
    public delegate void PlayerJoinedHandler(int playerNumber, PlayerTable playerTable);
    public static PlayerJoinedHandler PlayerJoined;
    public delegate void BeerBoxCompletedHandler(int playerNumber, int boxesCompleted);
    public static BeerBoxCompletedHandler BeerBoxCompleted;

    public delegate void BeerBoxRuinedHandler(int playerNumber);
    public static BeerBoxRuinedHandler BeerBoxRuined;

    public delegate void BeerBoxPowerUpHandler(int playerNumber, int increase);
    public static BeerBoxPowerUpHandler BeerBoxPowerUp;

    public delegate void BellRingedHandler(int boxesCompleted, float boxCompletedTime);
    public BellRingedHandler BellRinged;


    private const int ROWS_OF_BOXES = 2;
    private const int COLUMNS_OF_BOXES = 3;

    [Header("Components")]
    [SerializeField]
    private Transform beerBoxesParent;
    [SerializeField]
    private BeerBox beerBoxPrefab;
    [SerializeField]
    private PlayArea playArea;
    [SerializeField]
    private BottlePieceProjectionPreview bottlePieceProjectionPreview;

    [Header("Parameters")]
    [SerializeField]
    private int playerNumber = 0;
    public int PlayerNumber { get => playerNumber; }

    [SerializeField]
    private AnimationCurve beerBoxSpawnTimeProgression;
    [SerializeField]
    private AnimationCurve beerBoxDestroyTimeProgression;
    [SerializeField]
    private AnimationCurve beerBoxRuinTimeProgression;
    [SerializeField]
    private AnimationCurve addBottleTimeProgression;


    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource boxRuinedAudioSource;
    [SerializeField]
    private AudioSource boxSpawnedAudioSource;

    private float difficulty;
    private float beerBoxSpawnTime;
    private float beerBoxDestroyTime;
    private float beerBoxRuinTime;
    private float addBottleTime;


    private BeerBox[] beerBoxes;

    public bool nextBoxIsPowerUp = false;
    int lastPowerUpLevel = 0;
    int rivalPlayerLives = 3;

    void Awake()
    {
        playArea.SetPlayerNumber(playerNumber);
        PlayerJoined?.Invoke(playerNumber, this);
    }

    private void OnRivalPlayerLifesChanged(int livesLeft)
    {
        rivalPlayerLives = livesLeft;
    }

    private void InitliasiseBeerBoxes()
    {
        beerBoxes = beerBoxesParent.GetComponentsInChildren<BeerBox>();
        if (beerBoxes != null)
        {
            for (int i = 0; i < beerBoxes.Length; i++)
            {
                if (beerBoxes[i] != null)
                {
                    Destroy(beerBoxes[i].gameObject);

                }
            }
        }
        beerBoxes = new BeerBox[ROWS_OF_BOXES * COLUMNS_OF_BOXES];

        for (int i = 0; i < COLUMNS_OF_BOXES; i++)
        {
            for (int j = 0; j < ROWS_OF_BOXES; j++)
            {
                Vector3 spawnDirection = Vector3.forward;
                if (j == 0)
                {
                    spawnDirection = Vector3.back;
                }
                Vector3 boxPosition = beerBoxesParent.position + new Vector3(i * BeerBox.WIDTH, 0f, -j * BeerBox.DEPTH);
                beerBoxes[j * COLUMNS_OF_BOXES + i] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
                beerBoxes[j * COLUMNS_OF_BOXES + i].Spawn(spawnDirection);
            }
        }
    }

    private void OnDifficultyChanged(float newDifficulty, int newLevelDisplayNumber)
    {
        difficulty = newDifficulty;
        beerBoxSpawnTime = beerBoxSpawnTimeProgression.Evaluate(difficulty);
        beerBoxDestroyTime = beerBoxDestroyTimeProgression.Evaluate(difficulty);
        beerBoxRuinTime = beerBoxRuinTimeProgression.Evaluate(difficulty);
        addBottleTime = addBottleTimeProgression.Evaluate(difficulty);

        if (difficulty > 0 && newLevelDisplayNumber > lastPowerUpLevel)
        {
            nextBoxIsPowerUp = true;
            lastPowerUpLevel = newLevelDisplayNumber;
        }

        foreach (BeerBox beerBox in beerBoxes)
        {
            if (beerBox != null)
            {
                beerBox.UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime, beerBoxRuinTime);
            }
        }
    }


    private void OnDifficultyChangedByPowerUp(float newDifficulty)
    {
        difficulty = newDifficulty;
        beerBoxSpawnTime = beerBoxSpawnTimeProgression.Evaluate(difficulty);
        beerBoxDestroyTime = beerBoxDestroyTimeProgression.Evaluate(difficulty);
        beerBoxRuinTime = beerBoxRuinTimeProgression.Evaluate(difficulty);
        addBottleTime = addBottleTimeProgression.Evaluate(difficulty);

        foreach (BeerBox beerBox in beerBoxes)
        {
            if (beerBox != null)
            {
                beerBox.UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime, beerBoxRuinTime);
            }
        }
    }

    private void OnPieceDropped(BottlePiece bottlePiece)
    {
        StartCoroutine(DropPieceAndSpawnNewOne(bottlePiece));
    }


    private bool TryGetBeerBox(Vector3 position, out BeerBox beerBox)
    {
        beerBox = null;
        if (Physics.Raycast(position, Vector3.down, out RaycastHit RaycastHit, 100f, LayerManager.PieceDropLayerMask, QueryTriggerInteraction.Collide))
        {
            if (RaycastHit.collider.TryGetComponent<BeerBox>(out beerBox))
            {
                return true;
            }
        }
        return false;
    }


    private IEnumerator DropPieceAndSpawnNewOne(BottlePiece bottlePiece)
    {
        InputManager.Instance.PauseInputs(playerNumber, true);


        BeerBottle[] beerBottles = bottlePiece.GetBottles();
        List<BeerBox> fullBeerBoxes = new List<BeerBox>();


        IDictionary<BeerBox, int> beerBoxesToCollision = new Dictionary<BeerBox, int>();

        foreach (BeerBottle beerBottle in beerBottles)
        {
            if (TryGetBeerBox(beerBottle.GetPosition(), out BeerBox beerBox))
            {
                Vector2Int localCoordinatesInBeerBox = beerBox.GetLocalCoordinatesFromPoint(beerBottle.GetPosition());

                if (beerBox.IsClosestPositionEmpty(localCoordinatesInBeerBox))
                {
                    beerBox.AddBeerBottle(beerBottle, localCoordinatesInBeerBox);
                    if (beerBox.IsFull())
                    {
                        fullBeerBoxes.Add(beerBox);

                    }
                }
                else
                {
                    Debug.Log("No empty space found in beer box. Remove a life");
                    if (!beerBoxesToCollision.ContainsKey(beerBox))
                    {
                        beerBoxesToCollision[beerBox] = 0;
                    }
                    beerBoxesToCollision[beerBox]++;

                }
            }
            else
            {
                Debug.Log("No beerbox found for bottle. Remove a life");
            }
        }
        yield return new WaitForSeconds(addBottleTime);
        bottlePieceProjectionPreview.DisableAllProjections();
        bottlePieceProjectionPreview.DisableNextProjectionUpdateAnimation();

        Destroy(bottlePiece.gameObject);

        if (beerBoxesToCollision.Count > 0)
        {

            BeerBox highestCountBeerBox = null;
            int highestCount = -1;

            IDictionary<Vector3Int, Vector3> newBeerBoxIndexToSpawnDirection = new Dictionary<Vector3Int, Vector3>();

            foreach (BeerBox beerBox in beerBoxesToCollision.Keys)
            {
                if (beerBoxesToCollision[beerBox] > highestCount)
                {
                    highestCountBeerBox = beerBox;
                    highestCount = beerBoxesToCollision[beerBox];
                }
            }


            for (int i = 0; i < beerBoxes.Length; i++)
            {
                if (beerBoxes[i] == highestCountBeerBox)
                {
                    if (fullBeerBoxes.Contains(highestCountBeerBox))
                    {
                        fullBeerBoxes.Remove(highestCountBeerBox);
                    }
                    int columnIndex = i % COLUMNS_OF_BOXES;
                    int rowIndex = i / COLUMNS_OF_BOXES;

                    Vector3 clearingBoxDirection = Vector3.back;
                    if (rowIndex == 0)
                    {
                        clearingBoxDirection = Vector3.forward;
                    }

                    newBeerBoxIndexToSpawnDirection[new Vector3Int(i, columnIndex, rowIndex)] = -clearingBoxDirection;
                    beerBoxes[i].RuinAndDestroy(clearingBoxDirection);
                    beerBoxes[i] = null;
                }
            }

            BeerBoxRuined?.Invoke(playerNumber);

            boxRuinedAudioSource.pitch = Random.Range(0.95f, 1.05f);
            boxRuinedAudioSource.Play();
            HapticManager.Instance.PlayRigidImpact();
            yield return new WaitForSeconds(beerBoxRuinTime);

            foreach (Vector3Int beerBoxIndex in newBeerBoxIndexToSpawnDirection.Keys)
            {
                Vector3 boxPosition = beerBoxesParent.position + new Vector3(beerBoxIndex.y * BeerBox.WIDTH, 0f, -beerBoxIndex.z * BeerBox.DEPTH);
                beerBoxes[beerBoxIndex.x] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
                beerBoxes[beerBoxIndex.x].UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime, beerBoxRuinTime);
                beerBoxes[beerBoxIndex.x].Spawn(newBeerBoxIndexToSpawnDirection[beerBoxIndex]);

            }

            yield return new WaitForSeconds(beerBoxSpawnTime);
        }
        beerBoxesToCollision.Clear();

        if (fullBeerBoxes.Count > 0)
        {
            if (BellRinged != null)
            {
                BellRinged(fullBeerBoxes.Count, beerBoxDestroyTime);
            }

            IDictionary<Vector3Int, Vector3> newBeerBoxesIndexToSpawnDirection = new Dictionary<Vector3Int, Vector3>();
            for (int i = 0; i < beerBoxes.Length; i++)
            {
                if (fullBeerBoxes.Contains(beerBoxes[i]))
                {
                    int columnIndex = i % COLUMNS_OF_BOXES;
                    int rowIndex = i / COLUMNS_OF_BOXES;

                    Vector3 clearingBoxDirection = Vector3.back;
                    if (rowIndex == 0)
                    {
                        clearingBoxDirection = Vector3.forward;
                    }

                    newBeerBoxesIndexToSpawnDirection[new Vector3Int(i, columnIndex, rowIndex)] = -clearingBoxDirection;
                    beerBoxes[i].CompleteAndDestroy(clearingBoxDirection);
                    if (beerBoxes[i].boxType == BeerBox.TypeOfBox.SlowDownTime)
                    {
                        BeerBoxPowerUp?.Invoke(playerNumber, 0);
                    }
                    else if (beerBoxes[i].boxType == BeerBox.TypeOfBox.SpeedUpTime)
                    {
                        if (PlayerNumber == 0)
                        {
                            BeerBoxPowerUp?.Invoke(1, 1);
                        }
                        else
                        {
                            BeerBoxPowerUp?.Invoke(0, 1);
                        }

                    }
                    beerBoxes[i] = null;

                    yield return new WaitForSeconds(beerBoxDestroyTime);
                }
            }

            BeerBoxCompleted?.Invoke(playerNumber, fullBeerBoxes.Count);


            foreach (Vector3Int beerBoxIndex in newBeerBoxesIndexToSpawnDirection.Keys)
            {
                Vector3 boxPosition = beerBoxesParent.position + new Vector3(beerBoxIndex.y * BeerBox.WIDTH, 0f, -beerBoxIndex.z * BeerBox.DEPTH);
                beerBoxes[beerBoxIndex.x] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
                beerBoxes[beerBoxIndex.x].UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime, beerBoxRuinTime);
                if (nextBoxIsPowerUp)
                {
                    nextBoxIsPowerUp = false;
                    if (InputManager.NUMBER_OF_PLAYERS > 1)
                    {
                        if (Random.Range(0, 100.0f) <= 50)
                        {
                            beerBoxes[beerBoxIndex.x].boxType = BeerBox.TypeOfBox.SlowDownTime;
                        }
                        else
                        {
                            if (rivalPlayerLives <= 0)
                            {
                                beerBoxes[beerBoxIndex.x].boxType = BeerBox.TypeOfBox.SlowDownTime;

                            }
                            else
                            {
                                beerBoxes[beerBoxIndex.x].boxType = BeerBox.TypeOfBox.SpeedUpTime;
                            }
                        }
                    }
                    else
                    {
                        beerBoxes[beerBoxIndex.x].boxType = BeerBox.TypeOfBox.SlowDownTime;
                    }
                }
                beerBoxes[beerBoxIndex.x].Spawn(newBeerBoxesIndexToSpawnDirection[beerBoxIndex]);
            }
            boxSpawnedAudioSource.pitch = Random.Range(0.95f, 1.05f);
            boxSpawnedAudioSource.Play();
            yield return new WaitForSeconds(beerBoxSpawnTime);
        }
        fullBeerBoxes.Clear();


        playArea.SpawnNewPiece(PieceManager.Instance.GetRandomPiece(playerNumber));

        InputManager.Instance.PauseInputs(playerNumber, false);
    }

    internal void StartGame()
    {
        StopAllCoroutines();

        InitliasiseBeerBoxes();
        playArea.PieceDropped += OnPieceDropped;
        playArea.PieceMoved += OnPieceMoved;
        DifficultyManager.PlayerDifficultyChanged[playerNumber] += OnDifficultyChanged;
        DifficultyManager.PlayerDifficultyChangedByPowerUp[playerNumber] += OnDifficultyChangedByPowerUp;

        if (InputManager.NUMBER_OF_PLAYERS > 1)
        {
            if (playerNumber == 0)
            {
                DifficultyManager.PlayerLifesLeftChanged[1] += OnRivalPlayerLifesChanged;
            }
            else
            {
                DifficultyManager.PlayerLifesLeftChanged[0] += OnRivalPlayerLifesChanged;

            }
        }

        playArea.ListenToInputsEnableEvents(true);
        playArea.ClearPlayArea();
        playArea.StartPlayArea();
        playArea.SpawnNewPiece(PieceManager.Instance.GetRandomPiece(playerNumber));
    }

    internal void StopGame()
    {
        StopAllCoroutines();
        playArea.PieceDropped -= OnPieceDropped;
        playArea.PieceMoved -= OnPieceMoved;

        playArea.ListenToInputsEnableEvents(false);
        DifficultyManager.PlayerDifficultyChanged[playerNumber] -= OnDifficultyChanged;
        DifficultyManager.PlayerDifficultyChangedByPowerUp[playerNumber] -= OnDifficultyChangedByPowerUp;

        if (InputManager.NUMBER_OF_PLAYERS > 1)
        {
            if (playerNumber == 0)
            {
                DifficultyManager.PlayerLifesLeftChanged[1] -= OnRivalPlayerLifesChanged;
            }
            else
            {
                DifficultyManager.PlayerLifesLeftChanged[0] -= OnRivalPlayerLifesChanged;
            }
        }
    }

    private void OnPieceMoved(BottlePiece movedPiece, Vector3 newPiecePosition, float movementTime)
    {

        bool isInAValidPosition = true;
        Vector3 positionOffset = newPiecePosition - movedPiece.GetPosition();
        List<Vector3> bottlesPositions = new List<Vector3>();
        BeerBottle[] beerBottles = movedPiece.GetBottles();
        foreach (BeerBottle beerBottle in beerBottles)
        {
            Vector3 offesetBottlePosition = beerBottle.GetPosition() + positionOffset;
            bottlesPositions.Add(offesetBottlePosition);

            if (TryGetBeerBox(offesetBottlePosition, out BeerBox beerBox))
            {
                Vector2Int localCoordinatesInBeerBox = beerBox.GetLocalCoordinatesFromPoint(offesetBottlePosition);

                if (!beerBox.IsClosestPositionEmpty(localCoordinatesInBeerBox))
                {
                    isInAValidPosition = false;
                }
            }
            else
            {
                isInAValidPosition = false;
            }
        }

        bottlePieceProjectionPreview.UpdateProjection(isInAValidPosition, bottlesPositions, movementTime);
    }

    internal void SetPlayerControls(PlayerControls playerControls)
    {
        playArea.SetPlayerControls(playerControls);
    }
}

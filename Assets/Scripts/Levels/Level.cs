using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    public delegate void BeerBoxCompletedHandler(int boxesCompleted);
    public static BeerBoxCompletedHandler BeerBoxCompleted;

    public delegate void BellRingedHandler(int boxesCompleted, float boxCompletedTime);
    public BellRingedHandler BellRinged;

    public delegate void BeerBoxRuinedHandler();
    public static BeerBoxRuinedHandler BeerBoxRuined;

    private const int ROWS_OF_BOXES = 2;
    private const int COLUMNS_OF_BOXES = 3;

    [Header("Components")]
    [SerializeField]
    private Transform beerBoxesParent;
    [SerializeField]
    private BeerBox beerBoxPrefab;
    [SerializeField]
    private PlayArea playArea;

    [Header("Parameters")]
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

    private void InitliasiseBeerBoxes()
    {
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
                Vector3 boxPosition = new Vector3(i * BeerBox.WIDTH, 0f, -j * BeerBox.DEPTH);
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


    private bool TryGetBeerBox(BeerBottle beerBottle, out BeerBox beerBox)
    {
        Vector3 origin = beerBottle.GetPosition();
        beerBox = null;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit RaycastHit, 100f, LayerManager.PieceDropLayerMask, QueryTriggerInteraction.Collide))
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
        InputManager.Instance.PauseInputs(true);


        BeerBottle[] beerBottles = bottlePiece.GetBottles();
        List<BeerBox> fullBeerBoxes = new List<BeerBox>();


        IDictionary<BeerBox, int> beerBoxesToCollision = new Dictionary<BeerBox, int>();

        foreach (BeerBottle beerBottle in beerBottles)
        {
            if (TryGetBeerBox(beerBottle, out BeerBox beerBox))
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

            if (BeerBoxRuined != null)
            {
                BeerBoxRuined();
            }
            boxRuinedAudioSource.pitch = Random.Range(0.95f, 1.05f);
            boxRuinedAudioSource.Play();
            yield return new WaitForSeconds(beerBoxRuinTime);

            foreach (Vector3Int beerBoxIndex in newBeerBoxIndexToSpawnDirection.Keys)
            {
                Vector3 boxPosition = new Vector3(beerBoxIndex.y * BeerBox.WIDTH, 0f, -beerBoxIndex.z * BeerBox.DEPTH);
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
                    beerBoxes[i] = null;

                    yield return new WaitForSeconds(beerBoxDestroyTime);
                }
            }

            if (BeerBoxCompleted != null)
            {
                BeerBoxCompleted(fullBeerBoxes.Count);
            }

            foreach (Vector3Int beerBoxIndex in newBeerBoxesIndexToSpawnDirection.Keys)
            {
                Vector3 boxPosition = new Vector3(beerBoxIndex.y * BeerBox.WIDTH, 0f, -beerBoxIndex.z * BeerBox.DEPTH);
                beerBoxes[beerBoxIndex.x] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
                beerBoxes[beerBoxIndex.x].UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime, beerBoxRuinTime);
                beerBoxes[beerBoxIndex.x].Spawn(newBeerBoxesIndexToSpawnDirection[beerBoxIndex]);
            }
            boxSpawnedAudioSource.pitch = Random.Range(0.95f, 1.05f);
            boxSpawnedAudioSource.Play();
            yield return new WaitForSeconds(beerBoxSpawnTime);
        }
        fullBeerBoxes.Clear();


        playArea.SpawnNewPiece(PieceManager.Instance.GetRandomPiece());

        InputManager.Instance.PauseInputs(false);
    }

    internal void StartGame()
    {
        StopAllCoroutines();

        InitliasiseBeerBoxes();
        playArea.PieceDropped += OnPieceDropped;
        DifficultyManager.DifficultyChanged += OnDifficultyChanged;
        playArea.ClearPlayArea();
        playArea.StartPlayArea();
        playArea.SpawnNewPiece(PieceManager.Instance.GetRandomPiece());
    }

    internal void StopGame()
    {
        StopAllCoroutines();
        playArea.PieceDropped -= OnPieceDropped;
        DifficultyManager.DifficultyChanged -= OnDifficultyChanged;
    }
}

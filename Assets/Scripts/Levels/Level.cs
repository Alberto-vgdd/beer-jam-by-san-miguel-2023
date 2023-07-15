using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public delegate void BeerBoxCompletedHandler();
    public static BeerBoxCompletedHandler BeerBoxCompleted;

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
    private AnimationCurve addBottleTimeProgression;

    private float difficulty;
    private float beerBoxSpawnTime;
    private float beerBoxDestroyTime;
    private float addBottleTime;


    private BeerBox[] beerBoxes;
    List<BeerBox> fullBeerBoxes;


    private void Awake()
    {
        beerBoxes = new BeerBox[ROWS_OF_BOXES * COLUMNS_OF_BOXES];
        fullBeerBoxes = new List<BeerBox>();

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

    void Start()
    {
        playArea.SpawnNewPiece(Instantiate<BottlePiece>(PieceManager.Instance.GetRandomBottlePiece()));
    }


    void OnEnable()
    {
        playArea.PieceDropped += OnPieceDropped;
        DifficultyManager.DifficultyChanged += OnDifficultyChanged;
    }

    void OnDisable()
    {
        playArea.PieceDropped -= OnPieceDropped;
        DifficultyManager.DifficultyChanged -= OnDifficultyChanged;
    }

    private void OnDifficultyChanged(float newDifficulty)
    {
        difficulty = newDifficulty;
        beerBoxSpawnTime = beerBoxSpawnTimeProgression.Evaluate(difficulty);
        beerBoxDestroyTime = beerBoxDestroyTimeProgression.Evaluate(difficulty);
        addBottleTime = addBottleTimeProgression.Evaluate(difficulty);

        foreach (BeerBox beerBox in beerBoxes)
        {
            if (beerBox != null)
            {
                beerBox.UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime);
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
        InputManager.PauseGameplayInputs(true);

        BeerBottle[] beerBottles = bottlePiece.GetBottles();
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
                }
            }
            else
            {
                Debug.Log("No beerbox found for bottle. Remove a life");
            }
        }
        yield return new WaitForSeconds(addBottleTime);

        Destroy(bottlePiece.gameObject);

        if (fullBeerBoxes.Count > 0)
        {
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
                }
            }

            if (BeerBoxCompleted != null)
            {
                BeerBoxCompleted();
            }
            yield return new WaitForSeconds(beerBoxDestroyTime);

            foreach (Vector3Int beerBoxIndex in newBeerBoxesIndexToSpawnDirection.Keys)
            {
                Vector3 boxPosition = new Vector3(beerBoxIndex.y * BeerBox.WIDTH, 0f, -beerBoxIndex.z * BeerBox.DEPTH);
                beerBoxes[beerBoxIndex.x] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
                beerBoxes[beerBoxIndex.x].UpdateProgressionTimes(beerBoxSpawnTime, beerBoxDestroyTime, addBottleTime);
                beerBoxes[beerBoxIndex.x].Spawn(newBeerBoxesIndexToSpawnDirection[beerBoxIndex]);
            }

            yield return new WaitForSeconds(beerBoxSpawnTime);
        }
        fullBeerBoxes.Clear();


        playArea.SpawnNewPiece(Instantiate<BottlePiece>(PieceManager.Instance.GetRandomBottlePiece()));

        InputManager.PauseGameplayInputs(false);
    }
}

using UnityEngine;

public class Level : MonoBehaviour
{
    private const int ROWS_OF_BOXES = 2;
    private const int COLUMNS_OF_BOXES = 3;
    [Header("Components")]
    [SerializeField]
    private Transform beerBoxesParent;
    [SerializeField]
    private BeerBox beerBoxPrefab;

    private BeerBox[] beerBoxes;


    private void Awake()
    {
        beerBoxes = new BeerBox[ROWS_OF_BOXES * COLUMNS_OF_BOXES];

        for (int i = 0; i < COLUMNS_OF_BOXES; i++)
        {
            for (int j = 0; j < ROWS_OF_BOXES; j++)
            {
                Vector3 boxPosition = new Vector3(i * BeerBox.WIDTH, 0f, -j * BeerBox.DEPTH);
                beerBoxes[j * COLUMNS_OF_BOXES + i] = Instantiate<BeerBox>(beerBoxPrefab, boxPosition, Quaternion.identity, beerBoxesParent);
            }
        }
    }

}

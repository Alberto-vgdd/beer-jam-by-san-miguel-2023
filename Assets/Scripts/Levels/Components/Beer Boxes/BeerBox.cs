using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBox : MonoBehaviour
{
    internal const float WIDTH = 0.6f;
    internal const float DEPTH = 0.9f;

    [Header("Components.")]
    [SerializeField]
    private BeerBoxAnimator beerBoxAnimator;
    [SerializeField]
    private Transform beerBottlesParent;
    [SerializeField]
    private Transform beerBottlesOriginsParent;
    [SerializeField]
    private Transform[] beerBottlesOrigins;

    [Header("Debug.")]
    [SerializeField]
    private BeerBottle beerBottles;

    private IDictionary<Vector2Int, BeerBottle> coordinatesToBottle;
    private IDictionary<Vector2Int, Transform> coordinatesToOrigin;



    void Awake()
    {
        coordinatesToBottle = new Dictionary<Vector2Int, BeerBottle>();
        coordinatesToOrigin = new Dictionary<Vector2Int, Transform>();
        foreach (Transform beerBottleOrigin in beerBottlesOrigins)
        {
            coordinatesToOrigin.Add(GetLocalCoordinatesFromPoint(beerBottleOrigin.position), beerBottleOrigin);
        }
    }

    private Vector2Int GetLocalCoordinatesFromPoint(Vector3 position)
    {

        Vector3 localPosition = beerBottlesOriginsParent.InverseTransformPoint(position);
        Vector2Int localCoordinates = new Vector2Int(Mathf.RoundToInt((localPosition.x - 0.15f) / 0.3f), Mathf.RoundToInt((localPosition.z + 0.15f) / 0.3f));
        return localCoordinates;
    }

    private Vector3 GetPositionFromLocalCoordinates(Vector2Int localCoordinates)
    {
        Vector3 localPosition = new Vector3((localCoordinates.x * 0.3f) + 0.15f, 0f, (localCoordinates.y * 0.3f) - 0.15f);
        Vector3 position = beerBottlesOriginsParent.TransformPoint(localPosition);
        return position;
    }
}

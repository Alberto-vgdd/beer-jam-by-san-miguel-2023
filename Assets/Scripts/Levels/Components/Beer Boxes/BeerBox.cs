using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeerBox : MonoBehaviour
{
    internal const float WIDTH = 0.6f;
    internal const float DEPTH = 0.9f;

    internal const float BEER_BOX_SPAWN_TIME = 0.3f;
    internal const float BEER_BOX_DESTROY_TIME = 0.3f;
    internal const float ADD_BOTTLE_TIME = 0.1f;

    [Header("Components.")]
    [SerializeField]
    private BeerBoxAnimator beerBoxAnimator;
    [SerializeField]
    private Transform beerBottlesParent;
    [SerializeField]
    private Transform beerBottlesOriginsParent;
    [SerializeField]
    private Transform[] beerBottlesOrigins;

    private IDictionary<Vector2Int, BeerBottle> coordinatesToBottle;
    private IDictionary<Vector2Int, Transform> coordinatesToOrigin;

    private float beerBoxSpawnTime;
    private float beerBoxDestroyTime;
    private float addBottleTime;



    void Awake()
    {
        coordinatesToBottle = new Dictionary<Vector2Int, BeerBottle>();
        coordinatesToOrigin = new Dictionary<Vector2Int, Transform>();
        foreach (Transform beerBottleOrigin in beerBottlesOrigins)
        {
            coordinatesToOrigin.Add(GetLocalCoordinatesFromPoint(beerBottleOrigin.position), beerBottleOrigin);
        }
    }

    internal Vector2Int GetLocalCoordinatesFromPoint(Vector3 position)
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

    internal bool IsClosestPositionEmpty(Vector2Int coordinatesInBeerBox)
    {
        return (!coordinatesToBottle.ContainsKey(coordinatesInBeerBox) || coordinatesToBottle[coordinatesInBeerBox] == null);
    }

    internal void AddBeerBottle(BeerBottle beerBottle, Vector2Int localCoordinatesInBeerBox)
    {
        beerBottle.transform.SetParent(beerBottlesParent);
        beerBottle.MoveToBox(coordinatesToOrigin[localCoordinatesInBeerBox].position, addBottleTime);
        coordinatesToBottle[localCoordinatesInBeerBox] = beerBottle;
        beerBottle.EnableBoxCollider(true);
    }

    internal bool IsFull()
    {
        if (coordinatesToBottle.Values.Count < beerBottlesOrigins.Length)
        {
            return false;
        }

        foreach (BeerBottle beerBottle in coordinatesToBottle.Values)
        {
            if (beerBottle == null)
            {
                return false;
            }
        }

        return true;
    }

    internal void Clear()
    {
        foreach (BeerBottle beerBottle in coordinatesToBottle.Values)
        {
            Destroy(beerBottle.gameObject);
        }
        coordinatesToBottle.Clear();
    }

    internal void Spawn(Vector3 spawnDirection)
    {
        beerBoxAnimator.MoveFrom(spawnDirection * -DEPTH, beerBoxSpawnTime);
    }

    internal void CompleteAndDestroy(Vector3 clearingBoxDirection)
    {
        DOTween.Sequence()
            .Append(beerBoxAnimator.MoveTo(clearingBoxDirection * DEPTH, beerBoxDestroyTime))
            .OnComplete(() => Destroy(this.gameObject));
    }

    internal void UpdateProgressionTimes(float newBeerBoxSpawnTime, float newBeerBoxDestroyTime, float newAddBottleTime)
    {
        beerBoxSpawnTime = newBeerBoxSpawnTime;
        beerBoxDestroyTime = newBeerBoxDestroyTime;
        addBottleTime = newAddBottleTime;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BottlePiece : MonoBehaviour
{

    private static Vector3[] ROTATIONS = { Vector3.up * 0f, Vector3.up * 90f, Vector3.up * 180f, Vector3.up * 270f };

    [Header("Components")]
    [SerializeField]
    private Transform beerBottlesParent;
    [SerializeField]
    private Transform rotationPivot;
    [SerializeField]
    private Transform originOffset;
    [SerializeField]
    private BoxCollider boundsBoxCollider;
    [SerializeField]
    private BoxCollider nextBoundsBoxCollider;

    private BeerBottle[] beerBottles;
    private int rotationIndex;

    private Tween positionAnimation;
    private Tween rotationAnimation;

    private float movementTime;
    private float rotationTime;



    internal void Initialise()
    {
        rotationIndex = 0;
        beerBottles = beerBottlesParent.GetComponentsInChildren<BeerBottle>();

        foreach (BeerBottle beerBottle in beerBottles)
        {
            beerBottle.EnableBoxCollider(false);
        }
    }


    internal void Rotate()
    {
        DOTweenUtils.CompleteTween(rotationAnimation);

        rotationIndex++;
        if (rotationIndex >= ROTATIONS.Length)
        {
            rotationIndex = 0;
        }

        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence = rotationSequence.Append(rotationPivot.DOLocalRotate(ROTATIONS[rotationIndex], rotationTime));

        // foreach (BeerBottle beerBottle in beerBottles)
        // {
        //     rotationSequence = rotationSequence.Join(beerBottle.transform.DORotate(Vector3.zero, rotationTime));

        // }

        rotationAnimation = rotationSequence;

    }

    internal Vector3 GetPosition()
    {
        return transform.position;
    }

    internal Vector3 GetHalfExtents()
    {
        return boundsBoxCollider.bounds.extents;
    }

    internal Vector3 GetCenter()
    {
        return boundsBoxCollider.bounds.center;
    }

    internal void MoveToLocalPosition(Vector3 newLocalPosition, bool animate)
    {
        if (animate)
        {
            DOTweenUtils.CompleteTween(positionAnimation);
            positionAnimation = transform.DOLocalMove(newLocalPosition, movementTime);
        }
        else
        {
            transform.localPosition = newLocalPosition;
        }
    }

    internal void CorrectRotationMoveToLocalPosition(Vector3 newLocalPosition, bool animate)
    {
        if (animate)
        {
            DOTweenUtils.CompleteTween(positionAnimation);
            positionAnimation = transform.DOLocalMove(newLocalPosition, rotationTime);
        }
        else
        {
            transform.localPosition = newLocalPosition;
        }
    }

    internal void PreviewNextRotation(out Vector3 centerAfterRotation, out Vector3 halfExtentsAfterRotation)
    {
        centerAfterRotation = nextBoundsBoxCollider.bounds.center;
        halfExtentsAfterRotation = nextBoundsBoxCollider.bounds.extents;
    }

    internal BeerBottle[] GetBottles()
    {
        return beerBottles;
    }

    internal Vector3 GetOriginOffset()
    {
        return originOffset.position;
    }

    internal void SetPieceTimes(float newMovementTime, float newRotationTime)
    {
        movementTime = newMovementTime;
        rotationTime = newRotationTime;
    }

    internal float GetMovementTime()
    {
        return movementTime;
    }

    internal float GetRotationTime()
    {
        return rotationTime;
    }

    internal int GetNumberOfBottles()
    {
        return beerBottles.Length;
    }

    internal void SetBottlesVisuals(BottleVisuals[] bottlesVisuals)
    {
        for (int i = 0; i < beerBottles.Length; i++)
        {
            beerBottles[i].SetBottleVisuals(bottlesVisuals[i]);
        }
    }

    internal IDictionary<Vector2Int, Sprite> GetPieceLocalCoordinatesToSprites()
    {
        IDictionary<Vector2Int, Sprite> localCoordinatesToSprites = new Dictionary<Vector2Int, Sprite>();
        foreach (BeerBottle beerBottle in beerBottles)
        {
            localCoordinatesToSprites[GetLocalCoordinatesFromPoint(beerBottle.GetPosition())] = beerBottle.GetBottleVisuals().GetSprite();
        }
        return localCoordinatesToSprites;
    }


    internal Vector2Int GetLocalCoordinatesFromPoint(Vector3 position)
    {

        Vector3 localPosition = transform.InverseTransformPoint(position);
        Vector2Int localCoordinates = new Vector2Int(Mathf.RoundToInt((localPosition.x) / 0.3f), Mathf.RoundToInt((localPosition.z) / 0.3f));
        return localCoordinates;
    }
}

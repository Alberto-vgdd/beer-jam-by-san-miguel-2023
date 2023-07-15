using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BottlePiece : MonoBehaviour
{

    private static Vector3[] ROTATIONS = { Vector3.up * 0f, Vector3.up * 90f, Vector3.up * 180f, Vector3.up * 270f };
    public const float ROTATION_TIME = 0.075f;
    public const float MOVEMENT_TIME = 0.05f;


    [Header("Components")]
    [SerializeField]
    private Transform beerBottlesParent;
    [SerializeField]
    private Transform rotationPivot;
    [SerializeField]
    private BoxCollider boundsBoxCollider;
    [SerializeField]
    private BoxCollider nextBoundsBoxCollider;

    private BeerBottle[] beerBottles;
    private PlayerControls playerControls;
    private int rotationIndex;

    private Tween positionAnimation;
    private Tween rotationAnimation;




    // Start is called before the first frame update
    void Awake()
    {
        rotationIndex = 0;
        beerBottles = beerBottlesParent.GetComponentsInChildren<BeerBottle>();

        playerControls = new PlayerControls();
        playerControls.Enable();
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
        rotationSequence = rotationSequence.Append(rotationPivot.DOLocalRotate(ROTATIONS[rotationIndex], ROTATION_TIME));

        foreach (BeerBottle beerBottle in beerBottles)
        {
            rotationSequence = rotationSequence.Join(beerBottle.transform.DORotate(Vector3.zero, ROTATION_TIME));

        }

        rotationAnimation = rotationSequence;

    }

    internal Vector3 GetPosition()
    {
        return transform.position;
    }

    internal Vector3 GetHalfExtents()
    {
        return boundsBoxCollider.bounds.extents / 2f;
    }

    internal Vector3 GetCenter()
    {
        return boundsBoxCollider.bounds.center;
    }

    internal void MoveTo(Vector3 newPosition, float time)
    {
        DOTweenUtils.CompleteTween(positionAnimation);
        positionAnimation = transform.DOMove(newPosition, time);
    }

    internal void PreviewNextRotation(out Vector3 centerAfterRotation, out Vector3 halfExtentsAfterRotation)
    {
        centerAfterRotation = nextBoundsBoxCollider.bounds.center;
        halfExtentsAfterRotation = nextBoundsBoxCollider.bounds.extents / 2f;
    }
}

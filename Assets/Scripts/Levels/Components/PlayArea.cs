using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayArea : MonoBehaviour
{
    private const float PLAY_AREA_WIDTH = 1.8f;
    private const float PLAY_AREA_DEPTH = 1.8f;
    [Header("Components")]
    [SerializeField]
    private BottlePiece bottlePiece;

    private PlayerControls playerControls;


    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    void Start()
    {
        StartCoroutine(HandleInputs());
    }


    void OnEnable()
    {
        playerControls.Gameplay.Movement.performed += OnDpadPressed;
        playerControls.Gameplay.Rotate.performed += OnRotateButtonPressed;
    }

    void OnDisable()
    {
        playerControls.Gameplay.Movement.performed -= OnDpadPressed;
        playerControls.Gameplay.Rotate.performed -= OnRotateButtonPressed;
    }

    private void OnRotateButtonPressed(InputAction.CallbackContext context)
    {
        pendingRotation = true;
    }

    bool pendingRotation = false;
    IEnumerator HandleInputs()
    {
        while (true)
        {
            if (pendingRotation)
            {
                bottlePiece.Rotate();
                yield return new WaitForSeconds(BottlePiece.ROTATION_TIME);

                if (IsPieceOutOfBounds(bottlePiece.GetCenter(), bottlePiece.GetHalfExtents(), out Vector3 correctionOffset))
                {
                    bottlePiece.MoveTo(bottlePiece.GetPosition() + correctionOffset);
                }

                pendingRotation = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDpadPressed(CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.x != 0)
        {
            if (PieceCanBeMovedTo(bottlePiece, Vector3.right * input.x, out Vector3 candidatePosition))
            {
                bottlePiece.MoveTo(candidatePosition);
            }
        }

        if (input.y != 0)
        {
            if (PieceCanBeMovedTo(bottlePiece, Vector3.forward * input.y, out Vector3 candidatePosition))
            {
                bottlePiece.MoveTo(candidatePosition);
            }
        }

    }

    private bool PieceCanBeMovedTo(BottlePiece bottlePiece, Vector3 direction, out Vector3 candidatePosition)
    {
        float movementDistance = BeerBottle.BOTTLE_WIDTH;
        Vector3 position = bottlePiece.GetPosition();
        candidatePosition = position + (direction * movementDistance);


        Vector3 center = bottlePiece.GetCenter();
        Vector3 halfExtents = bottlePiece.GetHalfExtents();
        Vector3 newBoundsPosition = center + (direction * movementDistance);

        return !IsPieceOutOfBounds(newBoundsPosition, halfExtents, out Vector3 correctionOffset);
    }

    private bool IsPieceOutOfBounds(Vector3 pieceCenter, Vector3 halfExtents, out Vector3 correctionOffset)
    {
        correctionOffset = Vector3.zero;

        if (pieceCenter.x + halfExtents.x > PLAY_AREA_WIDTH)
        {
            correctionOffset += Vector3.left * BeerBottle.BOTTLE_WIDTH;
        }
        else if (pieceCenter.x - halfExtents.x < 0)
        {
            correctionOffset += Vector3.right * BeerBottle.BOTTLE_WIDTH;
        }
        if (pieceCenter.z + halfExtents.z > 0)
        {
            correctionOffset += Vector3.back * BeerBottle.BOTTLE_WIDTH;
        }
        else if (pieceCenter.z - halfExtents.z < -PLAY_AREA_DEPTH)
        {
            correctionOffset += Vector3.forward * BeerBottle.BOTTLE_WIDTH;
        }

        return correctionOffset != Vector3.zero;
    }
}

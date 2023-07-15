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
    private static Vector3 POSITION_CENTER_OFFSET = new Vector3(BeerBottle.BOTTLE_HALF_WIDTH, 0f, BeerBottle.BOTTLE_HALF_WIDTH);

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

    private void OnDpadPressed(CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        pendingInputs.Add(input);
    }

    bool pendingRotation = false;
    List<Vector2> pendingInputs = new List<Vector2>();

    IEnumerator HandleInputs()
    {
        while (true)
        {
            if (pendingRotation)
            {
                bottlePiece.PreviewNextRotation(out Vector3 centerAfterRotation, out Vector3 halfExtentsAfterRotation);

                if (IsPieceOutOfBounds(centerAfterRotation, halfExtentsAfterRotation, out Vector3 correctionOffset))
                {
                    bottlePiece.MoveTo(SnapPositionToGrid(bottlePiece.GetPosition() + correctionOffset), BottlePiece.ROTATION_TIME);
                }

                bottlePiece.Rotate();
                yield return new WaitForSeconds(BottlePiece.ROTATION_TIME);


                pendingRotation = false;
            }

            while (pendingInputs.Count > 0)
            {
                Vector2 input = pendingInputs[0];
                pendingInputs.RemoveAt(0);


                if (input.x != 0)
                {
                    if (PieceCanBeMovedTo(bottlePiece, Vector3.right * Mathf.Sign(input.x), out Vector3 candidatePosition))
                    {
                        bottlePiece.MoveTo(SnapPositionToGrid(candidatePosition), BottlePiece.MOVEMENT_TIME);
                        yield return new WaitForSeconds(BottlePiece.MOVEMENT_TIME);
                    }
                }

                else if (input.y != 0)
                {
                    if (PieceCanBeMovedTo(bottlePiece, Vector3.forward * Mathf.Sign(input.y), out Vector3 candidatePosition))
                    {
                        bottlePiece.MoveTo(SnapPositionToGrid(candidatePosition), BottlePiece.MOVEMENT_TIME);
                        yield return new WaitForSeconds(BottlePiece.MOVEMENT_TIME);
                    }
                }
            }
            yield return new WaitForEndOfFrame();
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

        Vector3 centerOnPlayArea = transform.InverseTransformPoint(pieceCenter);

        if (centerOnPlayArea.x + halfExtents.x > PLAY_AREA_WIDTH)
        {
            correctionOffset += Vector3.left * BeerBottle.BOTTLE_WIDTH;
        }
        else if (centerOnPlayArea.x - halfExtents.x < 0)
        {
            correctionOffset += Vector3.right * BeerBottle.BOTTLE_WIDTH;
        }
        if (centerOnPlayArea.z + halfExtents.z > 0)
        {
            correctionOffset += Vector3.back * BeerBottle.BOTTLE_WIDTH;
        }
        else if (centerOnPlayArea.z - halfExtents.z < -PLAY_AREA_DEPTH)
        {
            correctionOffset += Vector3.forward * BeerBottle.BOTTLE_WIDTH;
        }

        return correctionOffset != Vector3.zero;
    }

    private Vector3 SnapPositionToGrid(Vector3 position)
    {
        Vector3 positionOnPlayArea = transform.InverseTransformPoint(position);
        Vector3 snappedPosition = new Vector3(MathF.Floor(positionOnPlayArea.x / BeerBottle.BOTTLE_WIDTH) * BeerBottle.BOTTLE_WIDTH, positionOnPlayArea.y, Mathf.Floor(positionOnPlayArea.z / BeerBottle.BOTTLE_WIDTH) * BeerBottle.BOTTLE_WIDTH);
        return transform.TransformPoint(snappedPosition + POSITION_CENTER_OFFSET);
    }
}

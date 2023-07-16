using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Random = UnityEngine.Random;

public class PlayArea : MonoBehaviour
{
    private const float PLAY_AREA_WIDTH = 1.8f;
    private const float PLAY_AREA_DEPTH = 1.8f;
    private const float START_HEIGHT = 1.8f;
    private static Vector3 START_PLANE_CENTER = new Vector3(PLAY_AREA_WIDTH / 2f, 0f, -PLAY_AREA_DEPTH / 2f);
    private static Vector3 POSITION_CENTER_OFFSET = new Vector3(BeerBottle.BOTTLE_HALF_WIDTH, 0f, BeerBottle.BOTTLE_HALF_WIDTH);

    public delegate void PieceDroppedHandler(BottlePiece bottlePiece);
    public PieceDroppedHandler PieceDropped;


    [Header("Components")]
    [SerializeField]
    private Transform heightTransform;
    [SerializeField]
    private Transform planeTransform;

    [Header("Parameters")]
    [SerializeField]
    private AnimationCurve gravityTimeProgression;
    [SerializeField]
    private AnimationCurve heightChangeTimeProgression;
    [SerializeField]
    private AnimationCurve rotationTimeProgression;
    [SerializeField]
    private AnimationCurve movementTimeProgression;


    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource pieceRotatedAudioSource;
    [SerializeField]
    private AudioSource piecePlacedAudioSource;

    private BottlePiece bottlePiece;
    private PlayerControls playerControls;

    private float currentHeight = START_HEIGHT;
    private bool gravityEnabled = false;
    private float gravityTime;
    private float gravityTimer = 0f;
    private float difficulty;
    private float heightChangeTime;
    private float movementTime;
    private float rotationTime;


    void Awake()
    {
        playerControls = new PlayerControls();
    }





    private void OnDifficultyChanged(float newDifficulty, int newLevelDisplayNumber)
    {
        difficulty = newDifficulty;

        gravityTime = gravityTimeProgression.Evaluate(difficulty);
        movementTime = movementTimeProgression.Evaluate(difficulty);
        rotationTime = rotationTimeProgression.Evaluate(difficulty);
        heightChangeTime = heightChangeTimeProgression.Evaluate(difficulty);


        linearGravity = (gravityTime < 0.00f);


        if (bottlePiece != null)
        {
            bottlePiece.SetPieceTimes(movementTime, rotationTime);
        }
    }

    private void OnInputsEnabled(bool newEnabled)
    {
        if (newEnabled)
        {
            playerControls.Enable();
        }
        else
        {
            playerControls.Disable();
        }
    }

    private void OnDropPieceButtonPressed(CallbackContext context)
    {
        pendingDropPiece = true;
    }

    private void OnRotateButtonPressed(InputAction.CallbackContext context)
    {
        rotationCooldwon = true;
        pendingRotation = true;
    }

    private void OnDpadPressed(CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        pendingInputs.Add(input);
    }

    bool pendingRotation = false;
    bool pendingDropPiece = false;
    List<Vector2> pendingInputs = new List<Vector2>();


    IEnumerator HandleInputs()
    {
        while (true)
        {
            if (pendingRotation)
            {
                pendingRotation = false;
                bottlePiece.PreviewNextRotation(out Vector3 centerAfterRotation, out Vector3 halfExtentsAfterRotation);

                if (IsPieceOutOfBounds(centerAfterRotation, halfExtentsAfterRotation, out Vector3 correctionOffset))
                {
                    bottlePiece.CorrectRotationMoveToLocalPosition(WorldPositionToGridLocalPosition(bottlePiece.GetPosition() + correctionOffset), true);
                }

                bottlePiece.Rotate();


                pieceRotatedAudioSource.pitch = Random.Range(0.95f, 1.05f);
                pieceRotatedAudioSource.Play();
                yield return new WaitForSeconds(bottlePiece.GetRotationTime());

            }


            while (pendingInputs.Count > 0)
            {
                Vector2 input = pendingInputs[0];
                pendingInputs.RemoveAt(0);


                if (input.x != 0)
                {
                    if (PieceCanBeMovedTo(bottlePiece, Vector3.right * Mathf.Sign(input.x), out Vector3 candidatePosition))
                    {
                        bottlePiece.MoveToLocalPosition(WorldPositionToGridLocalPosition(candidatePosition), true);
                        yield return new WaitForSeconds(bottlePiece.GetMovementTime());
                    }
                }

                else if (input.y != 0)
                {
                    if (PieceCanBeMovedTo(bottlePiece, Vector3.forward * Mathf.Sign(input.y), out Vector3 candidatePosition))
                    {
                        bottlePiece.MoveToLocalPosition(WorldPositionToGridLocalPosition(candidatePosition), true);
                        yield return new WaitForSeconds(bottlePiece.GetMovementTime());
                    }
                }
            }

            if (pendingDropPiece && PieceDropped != null)
            {
                pendingDropPiece = false;
                pendingRotation = false;
                pendingInputs.Clear();
                DropPiece();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    bool linearGravity = false;
    bool rotationCooldwon = false;
    private IEnumerator HandleGravity()
    {
        while (true)
        {
            if (gravityEnabled)
            {
                gravityTimer += Time.deltaTime;

                if (rotationCooldwon)
                {
                    rotationCooldwon = false;

                    yield return new WaitForSeconds(0.2f);
                }

                if (linearGravity)
                {
                    heightTransform.localPosition = Vector3.Lerp(Vector3.up * START_HEIGHT, Vector3.up * 0.3f, gravityTimer / (gravityTime * 6f));
                    if (gravityTimer >= gravityTime * 6f)
                    {
                        gravityTimer = 0f;
                        gravityEnabled = false;
                        pendingDropPiece = true;
                    }
                }
                else
                {
                    if (gravityTimer >= gravityTime)
                    {
                        gravityTimer = 0f;

                        if (currentHeight <= BeerBottle.BOTTLE_WIDTH)
                        {
                            gravityEnabled = false;
                            pendingDropPiece = true;
                        }
                        else
                        {
                            currentHeight -= BeerBottle.BOTTLE_WIDTH;
                            heightTransform.DOLocalMove(Vector3.up * currentHeight, heightChangeTime);
                        }
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

        if (centerOnPlayArea.x + halfExtents.x > PLAY_AREA_WIDTH + 0.1f)
        {
            float magnitude = (centerOnPlayArea.x + halfExtents.x) - PLAY_AREA_WIDTH;
            correctionOffset += Vector3.left * magnitude;
        }
        else if (centerOnPlayArea.x - halfExtents.x < -0.1f)
        {
            float magnitude = -(centerOnPlayArea.x - halfExtents.x);
            correctionOffset += Vector3.right * magnitude;
        }
        if (centerOnPlayArea.z + halfExtents.z > 0.1f)
        {
            float magnitude = centerOnPlayArea.z + halfExtents.z;
            correctionOffset += Vector3.back * magnitude;
        }
        else if (centerOnPlayArea.z - halfExtents.z < -PLAY_AREA_DEPTH - 0.1f)
        {
            float magnitude = -PLAY_AREA_DEPTH - (centerOnPlayArea.z - halfExtents.z);
            correctionOffset += Vector3.forward * magnitude;
        }

        return correctionOffset != Vector3.zero;
    }



    private void DropPiece()
    {
        if (PieceDropped != null)
        {
            BottlePiece droppedPiece = bottlePiece;
            bottlePiece = null;
            gravityEnabled = false;
            PieceDropped(droppedPiece);
            piecePlacedAudioSource.pitch = Random.Range(0.95f, 1.05f);
            piecePlacedAudioSource.Play();
        }
    }

    internal void SpawnNewPiece(BottlePiece newBottlePiece)
    {
        bottlePiece = newBottlePiece;
        bottlePiece.transform.SetParent(planeTransform);
        bottlePiece.SetPieceTimes(movementTime, rotationTime);
        bottlePiece.MoveToLocalPosition(WorldPositionToGridLocalPosition(START_PLANE_CENTER), false);
        gravityEnabled = true;
        currentHeight = START_HEIGHT;
        gravityTimer = 0f;
        heightTransform.localPosition = Vector3.up * currentHeight;
    }

    private Vector3 WorldPositionToGridLocalPosition(Vector3 position)
    {
        Vector3 positionOnPlayArea = planeTransform.InverseTransformPoint(position);
        Vector3 snappedPosition = new Vector3(MathF.Floor(positionOnPlayArea.x / BeerBottle.BOTTLE_WIDTH) * BeerBottle.BOTTLE_WIDTH, 0f, Mathf.Floor(positionOnPlayArea.z / BeerBottle.BOTTLE_WIDTH) * BeerBottle.BOTTLE_WIDTH);
        return snappedPosition + POSITION_CENTER_OFFSET;
    }

    internal void ClearPlayArea()
    {
        StopAllCoroutines();

        playerControls.Disable();
        playerControls.Gameplay.Movement.performed -= OnDpadPressed;
        playerControls.Gameplay.Rotate.performed -= OnRotateButtonPressed;
        playerControls.Gameplay.DropPiece.performed -= OnDropPieceButtonPressed;

        DifficultyManager.DifficultyChanged -= OnDifficultyChanged;
        InputManager.InputEnabled -= OnInputsEnabled;

        if (bottlePiece != null)
        {
            Destroy(bottlePiece.gameObject);
        }

        gravityEnabled = true;
        currentHeight = START_HEIGHT;
        gravityTimer = 0f;
        heightTransform.localPosition = Vector3.up * currentHeight;
    }

    internal void StartPlayArea()
    {
        playerControls.Enable();
        playerControls.Gameplay.Movement.performed += OnDpadPressed;
        playerControls.Gameplay.Rotate.performed += OnRotateButtonPressed;
        playerControls.Gameplay.DropPiece.performed += OnDropPieceButtonPressed;

        DifficultyManager.DifficultyChanged += OnDifficultyChanged;
        InputManager.InputEnabled += OnInputsEnabled;

        StartCoroutine(HandleInputs());
        StartCoroutine(HandleGravity());
    }

}

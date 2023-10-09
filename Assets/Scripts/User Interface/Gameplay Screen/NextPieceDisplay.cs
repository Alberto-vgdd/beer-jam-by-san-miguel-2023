using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPieceDisplay : MonoBehaviour
{
    private const float BOTTLE_VISUAL_SIZE = 100f;

    [Header("Components")]
    [SerializeField]
    private RectTransform visualsParent;
    [SerializeField]
    private Image[] bottleVisualsImages;
    [SerializeField]
    private RectTransform[] bottleVisualsRectTransforms;
    [SerializeField]
    private GameObject[] bottleVisualsGameObjects;

    [Header("Parameters")]
    [SerializeField]
    private int playerNumber;

    void OnEnable()
    {
        PieceManager.PlayerNextPieceChanged[playerNumber] += OnNextPieceChanged;
    }

    void OnDisable()
    {
        PieceManager.PlayerNextPieceChanged[playerNumber] -= OnNextPieceChanged;
    }

    private void OnNextPieceChanged(BottlePiece bottlePiece)
    {
        IDictionary<Vector2Int, Sprite> localCoordinatesToSprites = bottlePiece.GetPieceLocalCoordinatesToSprites();
        foreach (GameObject bottleVisualGameObject in bottleVisualsGameObjects)
        {
            bottleVisualGameObject.SetActive(false);
        }

        int i = 0;

        Vector2 totalAnchoredPosition = Vector2.zero;
        foreach (Vector2Int localCoordinate in localCoordinatesToSprites.Keys)
        {
            bottleVisualsGameObjects[i].SetActive(true);
            bottleVisualsImages[i].sprite = localCoordinatesToSprites[localCoordinate];
            bottleVisualsRectTransforms[i].anchoredPosition = new Vector2(localCoordinate.x, localCoordinate.y) * BOTTLE_VISUAL_SIZE;
            totalAnchoredPosition += bottleVisualsRectTransforms[i].anchoredPosition;
            i++;
        }

        totalAnchoredPosition /= localCoordinatesToSprites.Keys.Count;
        visualsParent.anchoredPosition = -totalAnchoredPosition;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BottlePieceProjectionPreview : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Material validPositionMaterial;
    [SerializeField]
    private Material invalidPositionMaterial;
    [SerializeField]
    private Transform bottleProjectionsParent;

    [SerializeField]
    private Transform[] bottleProjections;
    [SerializeField]
    private MeshRenderer[] bottleProjectionsMaterials;

    private bool aniamteProjectionUpdate = false;

    private Tween updateProjectionAnimation;
    internal void UpdateProjection(bool isInAValidPosition, List<Vector3> bottlesPositions, float movementTime)
    {
        DOTweenUtils.CompleteTween(updateProjectionAnimation);
        Sequence animationSequence = DOTween.Sequence();

        DisableAllProjections();

        Vector3 bottlePosition;
        for (int i = 0; i < bottlesPositions.Count; i++)
        {
            bottleProjections[i].gameObject.SetActive(true);

            bottlePosition = bottlesPositions[i];
            bottlePosition.y = bottleProjectionsParent.position.y;

            if (aniamteProjectionUpdate)
            {
                animationSequence.Join(bottleProjections[i].DOMove(bottlePosition, movementTime));
            }
            else
            {
                bottleProjections[i].position = bottlePosition;
            }
            bottleProjectionsMaterials[i].sharedMaterial = isInAValidPosition ? validPositionMaterial : invalidPositionMaterial;
        }

        updateProjectionAnimation = animationSequence;
        aniamteProjectionUpdate = true;
    }

    public void DisableAllProjections()
    {
        for (int i = 0; i < bottleProjections.Length; i++)
        {
            bottleProjections[i].gameObject.SetActive(false);
        }
    }

    public void DisableNextProjectionUpdateAnimation()
    {
        aniamteProjectionUpdate = false;
    }
}

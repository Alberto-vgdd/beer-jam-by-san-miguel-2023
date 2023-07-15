using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeerBoxAnimator : MonoBehaviour
{
    [Header("Components.")]
    [SerializeField]
    private Transform positionPivot;
    [SerializeField]
    private Transform scalePivot;
    [SerializeField]
    private Transform rotationPivot;


    private Tween positionAnimation;


    internal Tween MoveFrom(Vector3 startPosition, float time)
    {
        DOTweenUtils.CompleteTween(positionAnimation);
        positionAnimation = positionPivot.DOLocalMove(startPosition, time).From();
        return positionAnimation;
    }

    internal Tween MoveTo(Vector3 targetPosition, float time)
    {
        DOTweenUtils.CompleteTween(positionAnimation);
        positionAnimation = positionPivot.DOLocalMove(targetPosition, time);
        return positionAnimation;
    }
}

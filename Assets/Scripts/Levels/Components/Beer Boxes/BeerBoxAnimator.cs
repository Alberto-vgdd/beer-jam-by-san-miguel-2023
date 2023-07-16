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


    [SerializeField]
    private ParticleSystem boxCompletedParticleSystem;
    [SerializeField]
    private ParticleSystem boxRuinedParticleSystem;

    private Tween positionAnimation;


    internal Tween MoveFrom(Vector3 startPosition, float time)
    {
        DOTweenUtils.CompleteTween(positionAnimation);
        positionAnimation = positionPivot.DOLocalMove(startPosition, time).From();
        return positionAnimation;
    }

    internal Tween MoveTo(Vector3 targetPosition, float time)
    {
        Vector3 direction = (targetPosition.x - transform.position.z > 0) ? Vector3.right : Vector3.left;
        DOTweenUtils.CompleteTween(positionAnimation);
        positionAnimation = DOTween.Sequence()
            .AppendCallback(boxCompletedParticleSystem.Play)
            .Append(positionPivot.DOLocalMoveY(0.2f, time / 2f).SetEase(Ease.InOutBack))
            .Join(scalePivot.DOPunchScale(new Vector3(-0.3f, 0.3f, -0.3f), time / 2f, 5, 0.5f))
            .Join(rotationPivot.DOLocalRotate(direction * 5f, time / 2f).SetEase(Ease.InOutBack))
            .AppendInterval(time / 4f)
            .Append(positionPivot.DOLocalMove(targetPosition, time / 4f))
            .Join(scalePivot.DOScale(Vector3.zero, time / 4f).SetEase(Ease.InBack));




        return positionAnimation;
    }

    internal Tween RuinAnimation(float beerBoxRuinTime)
    {
        DOTweenUtils.CompleteTween(positionAnimation);

        Vector3 randomRotation = new Vector3((UnityEngine.Random.value > 0.5) ? 5f : -5f, (UnityEngine.Random.value > 0.5) ? 6f : -6f, (UnityEngine.Random.value > 0.5) ? 4f : -4f);

        return DOTween.Sequence()
            .Append(
                DOTween.Sequence()
                .Append(scalePivot.DOPunchScale(new Vector3(0.8f, -0.6f, 0.8f), beerBoxRuinTime / 4f))
                    .AppendCallback(boxRuinedParticleSystem.Play)
                .Append(rotationPivot.DOLocalRotate(randomRotation, beerBoxRuinTime / 4f).SetEase(Ease.InOutBack))
                .AppendInterval(beerBoxRuinTime / 4f)
                .Append(scalePivot.DOScale(Vector3.zero, beerBoxRuinTime / 4f))
            )
            .Join(
                DOTween.Sequence()
                    .AppendInterval(beerBoxRuinTime / 4f)
                    .Append(positionPivot.DOLocalJump(Vector3.zero, 0.8f, 1, 3f * beerBoxRuinTime / 4f))
            );
    }
}

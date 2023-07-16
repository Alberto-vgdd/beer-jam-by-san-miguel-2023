using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LifeContainer : MonoBehaviour
{
    [SerializeField]
    private Transform animationPivot;
    [SerializeField]
    private GameObject usedLife;
    [SerializeField]
    private GameObject unusedLife;

    private Tween lifeAnimation;
    private bool isLifeUsed;
    void Start()
    {
        isLifeUsed = false;
        usedLife.SetActive(isLifeUsed);
        unusedLife.SetActive(!isLifeUsed);
    }

    internal void UseLife()
    {
        if (!isLifeUsed)
        {
            isLifeUsed = true;
            DOTweenUtils.CompleteTween(lifeAnimation);
            lifeAnimation = DOTween.Sequence()
                .AppendInterval(0.4f)
               .AppendCallback(() =>
               {
                   usedLife.SetActive(isLifeUsed);
                   unusedLife.SetActive(!isLifeUsed);
               })
               .Append(animationPivot.DOPunchScale(Vector3.one * -0.5f, 0.4f, 10, 0.6f).SetEase(Ease.InOutBack));
        }

    }

    internal void RecoverLife()
    {
        if (isLifeUsed)
        {
            isLifeUsed = false;
            DOTweenUtils.CompleteTween(lifeAnimation);
            lifeAnimation = DOTween.Sequence()
                .AppendInterval(0.4f)
                .AppendCallback(() =>
                {
                    usedLife.SetActive(isLifeUsed);
                    unusedLife.SetActive(!isLifeUsed);
                })
                .Append(animationPivot.DOPunchScale(Vector3.one * 0.4f, 0.4f, 5, 0.2f).SetEase(Ease.InOutBack));
        }

    }
}

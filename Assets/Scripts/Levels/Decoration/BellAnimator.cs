using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BellAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private PlayerTable level;
    [SerializeField]
    private Transform scalePivot;
    [SerializeField]
    private Transform soundWavesPivot;
    [SerializeField]
    private AudioSource bellRingedAudioSource;


    private Tween bellAnimation;

    void Awake()
    {
        soundWavesPivot.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        level.BellRinged += OnBellRinged;
    }

    void OnDisable()
    {
        level.BellRinged -= OnBellRinged;
    }

    private void OnBellRinged(int boxesCompleted, float beerCompletedTime)
    {
        DOTweenUtils.CompleteTween(bellAnimation);
        bellAnimation = DOTween.Sequence()
            .SetLoops(boxesCompleted)
            .AppendCallback(() =>
            {
                soundWavesPivot.gameObject.SetActive(true);
                bellRingedAudioSource.pitch = Random.Range(0.95f, 1.05f);
                bellRingedAudioSource.Play();
                HapticManager.Instance.PlayMediumImpact();
            })
            .Append(scalePivot.DOPunchScale(new Vector3(0.4f, -0.5f, 0.4f) * Random.Range(0.8f, 1f), beerCompletedTime, 10, 0.2f))
            .Join(soundWavesPivot.DOShakePosition(beerCompletedTime * 0.7f, 0.14f, 10))
            .AppendCallback(() =>
            {
                soundWavesPivot.gameObject.SetActive(false);
            });
    }
}

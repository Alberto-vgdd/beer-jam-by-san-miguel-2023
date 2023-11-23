using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class ChefAnimationController : MonoBehaviour
{
    [TextArea]
    public string chefText;

    public Text chefTextField;

    public RectTransform chefSprite;

    public CanvasGroup textBackGround;

    Vector3 initialPos;

    public RectTransform showPos;

    private void Awake()
    {
        initialPos = chefSprite.anchoredPosition;
    }
    private void OnEnable()
    {
        DOTween.Sequence().Append(chefSprite.DOAnchorPosY(showPos.anchoredPosition.y, 1)).Append(textBackGround.DOFade(1, 1)).Append(chefTextField.DOText(chefText, 1.5f)).OnComplete(KillChef);
    }

    private void OnDisable()
    {
        chefSprite.anchoredPosition = initialPos;
        chefTextField.text = "";
        textBackGround.alpha = 0;
    }

    void KillChef()
    {
        DOTween.Sequence().PrependInterval(60).Append(textBackGround.DOFade(0, 0.5f)).Append(chefSprite.DOAnchorPosY(initialPos.y, 1f)).Append(chefTextField.DOText("", 0));

    }
}

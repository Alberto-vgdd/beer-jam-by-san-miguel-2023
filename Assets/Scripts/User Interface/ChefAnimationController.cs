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

    public Transform chefSprite;

    public CanvasGroup textBackGround;

    Transform initialPos;

    public Transform showPos;

    private void Awake()
    {
        initialPos = transform;
    }
    private void OnEnable()
    {
        DOTween.Sequence().Append(chefSprite.DOMoveY(showPos.transform.position.y, 1)).Append(textBackGround.DOFade(1, 1)).Append(chefTextField.DOText(chefText, 1.5f)).OnComplete(KillChef);
    }

    private void OnDisable()
    {
        chefSprite.transform.position = initialPos.position;
        chefTextField.text = "";
        textBackGround.alpha = 0;
    }

    void KillChef()
    {
        DOTween.Sequence().PrependInterval(60).Append(textBackGround.DOFade(0, 0.5f)).Append(chefSprite.DOMoveY(initialPos.transform.position.y, 1f)).Append(chefTextField.DOText("", 0));

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PowerUpIconController : MonoBehaviour
{

    public Image background;
    public Image filledImage;


    public Sprite[] powerUpSprites;

    public Color[] powerUpColors;


    public int playerNumber;

    public CanvasGroup alertMsgCanvas;
    public Transform alertMsg;
    float startScale = 0;
    // Start is called before the first frame update
    void Start()
    {
        DifficultyManager.BeerBoxPowerUp[playerNumber] += OnStartPowerUp;

        startScale = alertMsg.localScale.x;
    }


    void OnStartPowerUp(float time, int increase) 
    {
        filledImage.fillAmount = 1;
        alertMsg.localScale = new Vector3(startScale,startScale,startScale);

        if (increase > 0)
        {
            background.sprite = powerUpSprites[1];
            filledImage.sprite = powerUpSprites[1];
            filledImage.color = powerUpColors[1];
            DOTween.Sequence().Append(alertMsgCanvas.DOFade(1, 0.25f)).Append(alertMsgCanvas.DOFade(0, time - 0.25f));
            AlertAnimationScaleUpSpeedUp();
            alertMsg.GetComponent<TextMeshProUGUI>().text = "Estres!";
            alertMsg.GetComponent<TextMeshProUGUI>().color = powerUpColors[1];
        }
        else
        {
            background.sprite = powerUpSprites[0];
            filledImage.sprite = powerUpSprites[0];
            filledImage.color = powerUpColors[0];
            DOTween.Sequence().Append(alertMsgCanvas.DOFade(1, 0.25f)).Append(alertMsgCanvas.DOFade(0, 3));
            AlertAnimationScaleUpSpeedDown();
            alertMsg.GetComponent<TextMeshProUGUI>().text = "Relax~";
            alertMsg.GetComponent<TextMeshProUGUI>().color = powerUpColors[0];


        }
        background.gameObject.SetActive(true);
        filledImage.gameObject.SetActive(true);

        GetComponent<AudioSource>().Play();

        filledImage.DOFillAmount(0, time).OnComplete(DeactivateIcon);
    }

    void DeactivateIcon() 
    {
        background.gameObject.SetActive(false);
        filledImage.gameObject.SetActive(false);
        GetComponent<AudioSource>().Stop();
    }




    void AlertAnimationScaleUpSpeedUp() 
    {
        if (filledImage.fillAmount > 0) 
        {
            alertMsg.DOScale(alertMsg.localScale.x*1.5f, 0.25f).OnComplete(AlertAnimationScaleDownSpeedUp);
        }
    }


    void AlertAnimationScaleDownSpeedUp()
    {
        if (filledImage.fillAmount > 0)
        {
            alertMsg.DOScale(alertMsg.localScale.x * 0.5f, 0.25f).OnComplete(AlertAnimationScaleUpSpeedUp);
        }
    }
    
    void AlertAnimationScaleUpSpeedDown() 
    {
        if (filledImage.fillAmount > 0) 
        {
            alertMsg.DOScale(alertMsg.localScale.x * 1.5f, 2f).OnComplete(AlertAnimationScaleDownSpeedDown);
        }
    }


    void AlertAnimationScaleDownSpeedDown()
    {
        if (filledImage.fillAmount > 0)
        {
            alertMsg.DOScale(alertMsg.localScale.x * 0.5f, 1.5f);
        }
    }



}

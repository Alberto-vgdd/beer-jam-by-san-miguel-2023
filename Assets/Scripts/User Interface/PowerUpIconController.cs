using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PowerUpIconController : MonoBehaviour
{

    public Image background;
    public Image filledImage;


    public Sprite[] powerUpSprites;

    public Color[] powerUpColors;


    public int playerNumber;

    // Start is called before the first frame update
    void Start()
    {
        DifficultyManager.BeerBoxPowerUp[playerNumber] += OnStartPowerUp;
    }


    void OnStartPowerUp(float time, int increase) 
    {
        if (increase > 0)
        {
            background.sprite = powerUpSprites[1];
            filledImage.sprite = powerUpSprites[1];
            filledImage.color = powerUpColors[1];
        }
        else
        {
            background.sprite = powerUpSprites[0];
            filledImage.sprite = powerUpSprites[0];
            filledImage.color = powerUpColors[0];
        }
        filledImage.fillAmount = 1;
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



}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleVisuals : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private Sprite sprite;

    internal Sprite GetSprite()
    {
        return sprite;
    }

    internal void SetParent(Transform bottleVisualsParent)
    {
        transform.SetParent(bottleVisualsParent);
        transform.localPosition = Vector3.zero;
    }
}

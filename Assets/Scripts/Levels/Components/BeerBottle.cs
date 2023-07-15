using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    public static float BOTTLE_WIDTH = 0.3f;
    public static float BOTTLE_HALF_WIDTH = BOTTLE_WIDTH / 2f;

    [Header("Components")]
    [SerializeField]
    private BoxCollider boxCollider;

    internal Vector3 GetPosition()
    {
        return transform.position;
    }

    internal void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    internal Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }

    internal void SetLocalPosition(Vector3 newLocalPosition)
    {
        transform.localPosition = newLocalPosition;
    }

    internal void EnableBoxCollider(bool enabled)
    {
        boxCollider.enabled = enabled;
    }


    internal void MoveToBox(Vector3 newPosition, float time)
    {
        transform.DOMove(newPosition, time);
    }


}

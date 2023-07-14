using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    public static float BOTTLE_WIDTH = 0.3f;
    public static float BOTTLE_HALF_WIDTH = BOTTLE_WIDTH / 2f;

    internal Vector3 GetPosition()
    {
        return transform.position;
    }

    internal Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }

}

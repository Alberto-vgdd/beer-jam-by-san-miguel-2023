using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleVisuals : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Transform meshTransform;

    internal Sprite GetSprite()
    {
        return sprite;
    }

    internal Mesh GetMesh()
    {
        return mesh;
    }

    internal Transform GetMeshTransform()
    {
        return meshTransform;
    }


    internal void SetParent(Transform bottleVisualsParent)
    {
        transform.SetParent(bottleVisualsParent);
        transform.localPosition = Vector3.zero;
    }
}

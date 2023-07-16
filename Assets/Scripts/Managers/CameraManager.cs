using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private delegate void ShakeCameraTriggeredHandler(float duration, float magnitude, int numberOfBounces);
    private static ShakeCameraTriggeredHandler ShakeCameraTriggered;

    [Header("Components")]
    [SerializeField]
    private CameraShaker cameraShaker;

    private void Awake()
    {
        ShakeCameraTriggered += OnShakeCameraTriggered;
    }

    public static void ShakeCamera(float duration, float magnitude, int numberOfBounces)
    {
        if (ShakeCameraTriggered != null)
        {
            ShakeCameraTriggered(duration, magnitude, numberOfBounces);
        }
    }

    private void OnShakeCameraTriggered(float duration, float magnitude, int numberOfBounces)
    {
        cameraShaker.OnShakeCameraTriggered(duration, magnitude, numberOfBounces);
    }


}



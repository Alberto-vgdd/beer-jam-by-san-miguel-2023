using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddeableButtons : MonoBehaviour
{
    [SerializeField]
    private RuntimePlatform[] unsupportedPlatforms;

    void Start()
    {
        foreach (RuntimePlatform platform in unsupportedPlatforms)
        {
            if (Application.platform == platform)
            {
                gameObject.SetActive(false);
                break;
            }
        }

    }
}

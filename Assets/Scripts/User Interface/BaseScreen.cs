using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public delegate void SelectGameObjectRequestedHandler(GameObject newGameObjectToSelect);
    public static SelectGameObjectRequestedHandler SelectGameObjectRequested;
}

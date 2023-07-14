using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class DOTweenUtils
{
    public static void CompleteTween(Tween aTween)
    {
        if (aTween != null && aTween.IsActive())
        {
            aTween.Complete(true);
        }
    }
}

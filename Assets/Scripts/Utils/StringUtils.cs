using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class StringUtils
{
    private const string FONT_WEIGHT_START = "<font-weight={0}>";
    private const string FONT_WEIGHT_END = "</font-weight>";


    public static string FormatStringWithFontWeight(string someText, int fontWeight)
    {
        return String.Format(FONT_WEIGHT_START, fontWeight) + someText + FONT_WEIGHT_END;
    }


}

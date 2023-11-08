using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticManager : Singleton<HapticManager>
{
    public void PlaySelection()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    protected override HapticManager GetThis()
    {
        return this;
    }
}

using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticManager : Singleton<HapticManager>
{
    private const string HAPTICS_ENABLED = "haptics_enabled";

    public bool HapticsEnabled { get => hapticsEnabled; }

    private bool hapticsEnabled = true;


    void Start()
    {
        if (PlayerPrefs.HasKey(HAPTICS_ENABLED))
        {
            hapticsEnabled = PlayerPrefs.GetInt(HAPTICS_ENABLED, 1) == 1;
        }
        EnableHaptics(hapticsEnabled);
    }

    public void PlaySelection()
    {
        if (hapticsEnabled)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }
    }

    public void EnableHaptics(bool newHapticsEnabled)
    {
        hapticsEnabled = newHapticsEnabled;
        PlayerPrefs.SetInt(HAPTICS_ENABLED, hapticsEnabled ? 1 : 0);
    }


    protected override HapticManager GetThis()
    {
        return this;
    }
}

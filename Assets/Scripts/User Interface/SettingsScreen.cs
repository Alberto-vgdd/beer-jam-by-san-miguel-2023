using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : BaseScreen
{
    [Header("Haptics Components")]
    [SerializeField]
    private GameObject hapticsToggleSprite;
    [Header("Audio Components")]
    [SerializeField]
    private GameObject audioToggleSprite;

    void OnEnable()
    {
        hapticsToggleSprite.SetActive(HapticManager.Instance.HapticsEnabled);
        audioToggleSprite.SetActive(AudioManager.Instance.AudioEnabled);
    }

    public void OnHapticsToggleButtonPressed()
    {
        bool newHapticsEnabled = !HapticManager.Instance.HapticsEnabled;
        HapticManager.Instance.EnableHaptics(newHapticsEnabled);
        hapticsToggleSprite.SetActive(newHapticsEnabled);
    }

    public void OnAudioToggleButtonPressed()
    {
        bool newAudioEnabled = !AudioManager.Instance.AudioEnabled;
        AudioManager.Instance.EnableAudio(newAudioEnabled);
        audioToggleSprite.SetActive(newAudioEnabled);
    }
}

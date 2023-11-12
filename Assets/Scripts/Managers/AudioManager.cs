using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    private const string AUDIO_ENABLED = "audio_enabled";
    private const string AUDIO_VOLUME_PARAM = "audio_volume";
    private const float AUDIO_DEFAULT_VOLUME = 0f;
    private const float AUDIO_MUTED_VOLUME = -80f;

    public bool AudioEnabled { get => audioEnabled; }


    [Header("Components")]
    [SerializeField]
    private AudioMixer audioMixer;


    private bool audioEnabled = true;


    void Start()
    {
        if (PlayerPrefs.HasKey(AUDIO_ENABLED))
        {
            audioEnabled = PlayerPrefs.GetInt(AUDIO_ENABLED, 1) == 1;
        }
        EnableAudio(audioEnabled);
    }


    public void EnableAudio(bool newAudioEnabled)
    {
        audioEnabled = newAudioEnabled;
        audioMixer.SetFloat(AUDIO_VOLUME_PARAM, audioEnabled ? AUDIO_DEFAULT_VOLUME : AUDIO_MUTED_VOLUME);
        PlayerPrefs.SetInt(AUDIO_ENABLED, audioEnabled ? 1 : 0);
    }

    protected override AudioManager GetThis()
    {
        return this;
    }

}

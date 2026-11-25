using UnityEngine;
using UnityEngine.Audio;

public static class AudioVolumes
{
    public const float MinValue = 0.0001f;

    public static float Load(string param)
    {
        return PlayerPrefs.GetFloat(param, 0.8f);
    }

    public static void Apply(AudioMixer mixer, string param, float value)
    {
        // Log mapping: 1 = 0 dB, 0.5 = -6 dB. The bottom of the slider is a safe mute at -80 dB.
        float db = value <= MinValue ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(param, db);
    }

    public static void Save(string param, float value)
    {
        PlayerPrefs.SetFloat(param, value);
        PlayerPrefs.Save();
    }

    public static void ApplyAll(AudioMixer mixer)
    {
        Apply(mixer, "MasterVolume", Load("MasterVolume"));
        Apply(mixer, "MusicVolume", Load("MusicVolume"));
        Apply(mixer, "SfxVolume", Load("SfxVolume"));
        Apply(mixer, "UiVolume", Load("UiVolume"));
    }
}
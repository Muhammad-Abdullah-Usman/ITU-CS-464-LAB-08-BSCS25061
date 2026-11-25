using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider uiSlider;

    void Start()
    {
        Setup(masterSlider, "MasterVolume");
        Setup(musicSlider, "MusicVolume");
        Setup(sfxSlider, "SfxVolume");
        Setup(uiSlider, "UiVolume");

        masterSlider.onValueChanged.AddListener(SetMaster);
        musicSlider.onValueChanged.AddListener(SetMusic);
        sfxSlider.onValueChanged.AddListener(SetSfx);
        uiSlider.onValueChanged.AddListener(SetUi);
    }

    void Setup(Slider slider, string param)
    {
        slider.minValue = AudioVolumes.MinValue;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(AudioVolumes.Load(param));
    }

    void SetMaster(float v) { Change("MasterVolume", v); }
    void SetMusic(float v) { Change("MusicVolume", v); }
    void SetSfx(float v) { Change("SfxVolume", v); }
    void SetUi(float v) { Change("UiVolume", v); }

    void Change(string param, float v)
    {
        AudioVolumes.Apply(mixer, param, v);
        AudioVolumes.Save(param, v);
    }
}
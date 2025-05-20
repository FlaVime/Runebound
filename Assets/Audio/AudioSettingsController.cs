using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private const string VolumePrefKey = "MasterVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.HasKey(VolumePrefKey) ? PlayerPrefs.GetFloat(VolumePrefKey) : 0.3f;

        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MasterVolume", dB);

        PlayerPrefs.SetFloat(VolumePrefKey, sliderValue);
        PlayerPrefs.Save();
    }
}

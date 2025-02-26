using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuAudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MenuMasterVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("MenuSFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MenuMusicVolume", 1f);

        SetMasterVolume(masterVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MenuMasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MenuMasterVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("MenuSFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MenuSFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MenuMusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MenuMusicVolume", volume);
    }
}
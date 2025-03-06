using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameAudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider cutsceneVolumeSlider;
    public Slider playerSFXVolumeSlider;
    public Slider enemySFXVolumeSlider;
    public Slider pickupSFXVolumeSlider;
    public Slider generalSFXVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("GameMasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
        cutsceneVolumeSlider.value = PlayerPrefs.GetFloat("GameCutsceneVolume", 1f);
        playerSFXVolumeSlider.value = PlayerPrefs.GetFloat("GamePlayerSFXVolume", 1f);
        enemySFXVolumeSlider.value = PlayerPrefs.GetFloat("GameEnemySFXVolume", 1f);
        pickupSFXVolumeSlider.value = PlayerPrefs.GetFloat("GamePickUpSFXVolume", 1f);
        generalSFXVolumeSlider.value = PlayerPrefs.GetFloat("GameGeneralSFXVolume", 1f);

        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetCutsceneVolume(cutsceneVolumeSlider.value);
        SetPlayerSFXVolume(playerSFXVolumeSlider.value);
        SetEnemySFXVolume(enemySFXVolumeSlider.value);
        SetPickUpSFXVolume(pickupSFXVolumeSlider.value);
        SetGeneralSFXVolume(generalSFXVolumeSlider.value);

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        cutsceneVolumeSlider.onValueChanged.AddListener(SetCutsceneVolume);
        playerSFXVolumeSlider.onValueChanged.AddListener(SetPlayerSFXVolume);
        enemySFXVolumeSlider.onValueChanged.AddListener(SetEnemySFXVolume);
        pickupSFXVolumeSlider.onValueChanged.AddListener(SetPickUpSFXVolume);
        generalSFXVolumeSlider.onValueChanged.AddListener(SetGeneralSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("GameMasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameMasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("GameMusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameMusicVolume", volume);
    }

    public void SetCutsceneVolume(float volume)
    {
        audioMixer.SetFloat("GameCutsceneVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameCutsceneVolume", volume);
    }

    public void SetPlayerSFXVolume(float volume)
    {
        audioMixer.SetFloat("GamePlayerSFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GamePlayerSFXVolume", volume);
    }

    public void SetEnemySFXVolume(float volume)
    {
        audioMixer.SetFloat("GameEnemySFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameEnemySFXVolume", volume);
    }

    public void SetPickUpSFXVolume(float volume)
    {
        audioMixer.SetFloat("GamePickUpSFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GamePickUpSFXVolume", volume);
    }

    public void SetGeneralSFXVolume(float volume)
    {
        audioMixer.SetFloat("GameGeneralSFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameGeneralSFXVolume", volume);
    }
}
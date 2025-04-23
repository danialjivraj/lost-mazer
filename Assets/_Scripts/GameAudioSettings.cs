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
        if (masterVolumeSlider != null)
        {
            float m = PlayerPrefs.GetFloat("GameMasterVolume", 1f);
            masterVolumeSlider.value = m;
            SetMasterVolume(m);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            float m = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
            musicVolumeSlider.value = m;
            SetMusicVolume(m);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (cutsceneVolumeSlider != null)
        {
            float c = PlayerPrefs.GetFloat("GameCutsceneVolume", 1f);
            cutsceneVolumeSlider.value = c;
            SetCutsceneVolume(c);
            cutsceneVolumeSlider.onValueChanged.AddListener(SetCutsceneVolume);
        }

        if (playerSFXVolumeSlider != null)
        {
            float p = PlayerPrefs.GetFloat("GamePlayerSFXVolume", 1f);
            playerSFXVolumeSlider.value = p;
            SetPlayerSFXVolume(p);
            playerSFXVolumeSlider.onValueChanged.AddListener(SetPlayerSFXVolume);
        }

        if (enemySFXVolumeSlider != null)
        {
            float e = PlayerPrefs.GetFloat("GameEnemySFXVolume", 1f);
            enemySFXVolumeSlider.value = e;
            SetEnemySFXVolume(e);
            enemySFXVolumeSlider.onValueChanged.AddListener(SetEnemySFXVolume);
        }

        if (pickupSFXVolumeSlider != null)
        {
            float pu = PlayerPrefs.GetFloat("GamePickUpSFXVolume", 1f);
            pickupSFXVolumeSlider.value = pu;
            SetPickUpSFXVolume(pu);
            pickupSFXVolumeSlider.onValueChanged.AddListener(SetPickUpSFXVolume);
        }

        if (generalSFXVolumeSlider != null)
        {
            float g = PlayerPrefs.GetFloat("GameGeneralSFXVolume", 1f);
            generalSFXVolumeSlider.value = g;
            SetGeneralSFXVolume(g);
            generalSFXVolumeSlider.onValueChanged.AddListener(SetGeneralSFXVolume);
        }
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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Linq;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }
    const string PREF_KEY = "BrightnessExposure";

    PostProcessVolume currentVolume;
    ColorGrading grading;

    void Awake()
    {
    if (Instance == null)
    {
        Instance = this;
        var rootGO = transform.root.gameObject;
        DontDestroyOnLoad(rootGO);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else Destroy(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentVolume = FindObjectOfType<PostProcessVolume>();
        if (currentVolume != null && 
            currentVolume.profile.TryGetSettings(out grading))
        {
            grading.postExposure.value =
                PlayerPrefs.GetFloat(PREF_KEY, 0f);
        }

        if (scene.name == "MainMenu")
        {
            var slider = Resources
                .FindObjectsOfTypeAll<Slider>()
                .FirstOrDefault(s => s.name == "BrightnessSlider");

            if (slider == null)
            {
                Debug.LogError("BrightnessSlider not found in scene");
                return;
            }

            slider.onValueChanged.RemoveAllListeners();

            float saved = PlayerPrefs.GetFloat(PREF_KEY, 0f);
            slider.minValue = -2f;
            slider.maxValue = +2f;
            slider.SetValueWithoutNotify(saved);
            slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void OnSliderChanged(float v)
    {
        PlayerPrefs.SetFloat(PREF_KEY, v);
        PlayerPrefs.Save();
        if (grading != null)
            grading.postExposure.value = v;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

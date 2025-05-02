using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Linq;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }
    const string PREF_KEY = "BrightnessExposure";
    Slider brightnessSlider;

    PostProcessVolume currentVolume;
    ColorGrading     grading;

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
            var all = FindObjectsOfType<Slider>();
            brightnessSlider = all
              .FirstOrDefault(s => s.gameObject.name == "BrightnessSlider");

            if (brightnessSlider == null)
            {
                Debug.LogError("Could not find a Slider named 'BrightnessSlider'");
                return;
            }

            brightnessSlider.onValueChanged.RemoveAllListeners();

            float saved = PlayerPrefs.GetFloat(PREF_KEY, 0f);
            brightnessSlider.minValue = -2f;
            brightnessSlider.maxValue = +2f;
            brightnessSlider.SetValueWithoutNotify(saved);

            brightnessSlider.onValueChanged.AddListener(OnSliderChanged);
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

using UnityEngine;
using UnityEngine.UI;

public class ShowLanternToggle : MonoBehaviour
{
    const string PREF_KEY = "ShowLantern";
    public Toggle toggle;
    public GameObject lantern;

    void Start()
    {
        bool isOn = PlayerPrefs.GetInt(PREF_KEY, 1) == 1;

        toggle.SetIsOnWithoutNotify(isOn);

        lantern.SetActive(isOn);

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool show)
    {
        lantern.SetActive(show);

        PlayerPrefs.SetInt(PREF_KEY, show ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}

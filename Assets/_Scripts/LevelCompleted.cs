using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    public int currentLevel = 1;
    public string nextSceneName;
    public int levelToUnlock = 2;
    public GameObject fadeToBlack;

    private bool isTransitioning = false;

    void Start()
    {
        if (fadeToBlack != null)
            fadeToBlack.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartLevelCompletion();
    }

    public void StartLevelCompletion()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (fadeToBlack != null)
            fadeToBlack.SetActive(true);

        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

        ScoreManager.instance.SaveScore();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (levelToUnlock > unlockedLevel)
        {
            Debug.Log("Unlocked level: " + levelToUnlock);
            PlayerPrefs.SetInt("UnlockedLevel", levelToUnlock);
            PlayerPrefs.Save();
        }

        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(nextSceneName);
    }
}
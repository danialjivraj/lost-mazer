using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    public GameObject fadeFX;
    public string nextSceneName;
    public int levelToUnlock = 2;

    private bool isTransitioning = false;

    void Start()
    {
        if (fadeFX != null) fadeFX.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;

            if (fadeFX != null)
                fadeFX.SetActive(true);

            int currentLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            if (levelToUnlock > currentLevel)
            {
                Debug.Log("Unlocked level: " + levelToUnlock);
                PlayerPrefs.SetInt("UnlockedLevel", levelToUnlock);
                PlayerPrefs.Save();
            }

            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(nextSceneName);
    }
}
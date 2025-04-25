using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackScreenMenu : MonoBehaviour
{
    public string nextSceneName;
    public AudioSource buttonSound;

    public void Retry()
    {
        PlayClick();

        string lastLevel = LevelManager.GetLastLevel();
        if (!string.IsNullOrEmpty(lastLevel))
        {
            SceneManager.LoadScene(lastLevel);
        }
        else
        {
            Debug.LogError("No last level stored, you might need to start the game from the Main Menu scene");
        }
    }

    public void BackToMenu()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
    }

    public void GoToNextScene()
    {
        PlayClick();
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name not set on BlackScreenMenu");
        }
    }

    private void PlayClick()
    {
        if (buttonSound != null) buttonSound.Play();
    }
}

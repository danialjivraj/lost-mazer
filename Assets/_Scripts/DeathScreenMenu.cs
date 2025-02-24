using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenMenu : MonoBehaviour
{
    public void Retry()
    {
        string lastLevel = LevelManager.GetLastLevel();
        if (!string.IsNullOrEmpty(lastLevel))
        {
            SceneManager.LoadScene(lastLevel);
        }
        else
        {
            Debug.LogError("No last level stored!");
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
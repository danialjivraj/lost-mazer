using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuLogic : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject settingsMenu;
    private GameObject score;
    private GameObject level;
    public GameObject controls;
    public GameObject Audio;
    public AudioSource buttonSound;
    public Button[] levelButtons;
    public TMP_Text scoreTextLeft;
    public TMP_Text scoreTextRight;

    void Start()
    {
        mainMenu = GameObject.Find("MainMenuCanvas");
        settingsMenu = GameObject.Find("OptionsCanvas");
        score = GameObject.Find("ScoreCanvas");
        level = GameObject.Find("LevelCanvas");

        if (mainMenu == null) Debug.LogError("MainMenuCanvas not found!");
        if (settingsMenu == null) Debug.LogError("OptionsCanvas not found!");
        if (score == null) Debug.LogError("ScoreCanvas not found!");
        if (level == null) Debug.LogError("LevelCanvas not found!");

        mainMenu.GetComponent<Canvas>().enabled = true;
        settingsMenu.GetComponent<Canvas>().enabled = false;
        score.SetActive(false);
        level.SetActive(false);
        controls.SetActive(false);
        Audio.SetActive(false);

        LockLevels();
    }

    void LockLevels()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].interactable = levelIndex <= unlockedLevel;
        }
    }

    public void StartButton(string selectedSceneName)
    {
        mainMenu.GetComponent<Canvas>().enabled = false;

        if (buttonSound != null)
            buttonSound.Play();
        else
            Debug.LogWarning("Button sound not assigned!");

        if (!string.IsNullOrEmpty(selectedSceneName))
            SceneManager.LoadScene(selectedSceneName);
        else
            Debug.LogError("Scene name is empty!");
    }

    public void LevelButton()
    {
        buttonSound.Play();

        mainMenu.GetComponent<Canvas>().enabled = false;
        level.SetActive(true);
        controls.SetActive(false);
        score.SetActive(false);
        Audio.SetActive(true);
    }

    public void ScoreButton()
    {
        buttonSound.Play();

        mainMenu.GetComponent<Canvas>().enabled = false;
        score.SetActive(true);
        controls.SetActive(false);
        level.SetActive(false);
        Audio.SetActive(true);

        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        string leftScores = "";
        string rightScores = "";

        for (int i = 0; i < 5; i++)
        {
            int loadedScore = PlayerPrefs.GetInt("HighScore" + i, 0);
            leftScores += (i + 1) + ". " + loadedScore + " Coins\n";
        }

        for (int i = 5; i < 10; i++)
        {
            int loadedScore = PlayerPrefs.GetInt("HighScore" + i, 0);
            rightScores += (i + 1) + ". " + loadedScore + " Coins\n";
        }

        scoreTextLeft.text = leftScores;
        scoreTextRight.text = rightScores;
    }

    public void SettingsButton()
    {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = false;
        settingsMenu.GetComponent<Canvas>().enabled = true;
        controls.SetActive(false);
        Audio.SetActive(true);
    }

    public void ControlsButton()
    {
        controls.SetActive(true);
        Audio.SetActive(false);
        buttonSound.Play();
    }

    public void AudioButton()
    {
        controls.SetActive(false);
        Audio.SetActive(true);
        buttonSound.Play();
    }

    public void ExitGameButton()
    {
        buttonSound.Play();
        Application.Quit();
        Debug.Log("App Has Exited");
    }

    public void ReturnToMainMenuButton()
    {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = true;
        settingsMenu.GetComponent<Canvas>().enabled = false;
        score.SetActive(false);
        level.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuLogic : MonoBehaviour
{
    public GameObject continueGameCanvas;
    public TMP_Text continueGameText;

    public GameObject controlsCanvas;
    public GameObject audioCanvas;
    public GameObject brightnessCanvas;

    public Button[] levelButtons;
    public TMP_Text scoreTextLeft;
    public TMP_Text scoreTextRight;

    public AudioSource buttonSound;

    private GameObject mainMenuCanvas;
    private GameObject settingsCanvas;
    private GameObject scoreCanvas;
    private GameObject levelCanvas;

    public GameObject backgroundImage;
    public GameObject backgroundPreview;
    public Button controlsTabButton;
    public Button scoreLevel1TabButton;

    private string playerId;
    private int selectedLevel = 1;

    void Start()
    {
        AudioListener.pause = false;

        mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        settingsCanvas = GameObject.Find("OptionsCanvas");
        scoreCanvas = GameObject.Find("ScoreCanvas");
        levelCanvas = GameObject.Find("LevelCanvas");

        if (!mainMenuCanvas) Debug.LogError("MainMenuCanvas not found");
        if (!settingsCanvas) Debug.LogError("OptionsCanvas not found");
        if (!scoreCanvas) Debug.LogError("ScoreCanvas not found");
        if (!levelCanvas) Debug.LogError("LevelCanvas not found");

        mainMenuCanvas.GetComponent<Canvas>().enabled = true;
        settingsCanvas.GetComponent<Canvas>().enabled = false;
        scoreCanvas.SetActive(false);
        levelCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
        audioCanvas.SetActive(false);
        brightnessCanvas.SetActive(false);
        continueGameCanvas.SetActive(false);

        playerId = PlayerIdManager.instance.GetPlayerId();

        LockLevels();
        RefreshScoreDisplay();
    }

    void LockLevels()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < levelButtons.Length; i++)
            levelButtons[i].interactable = (i+1) <= unlockedLevel;
    }

    // buttons
    public void StartButton(string sceneName)
    {
        PlayClick();
        mainMenuCanvas.GetComponent<Canvas>().enabled = false;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty");
            return;
        }

        var digits = new string(sceneName.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out int lvl))
            PlayerPrefs.SetInt("CurrentLevel", lvl);

        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void ContinueGameYes()
    {
        PlayClick();
        var data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            int lvl = PlayerPrefs.GetInt("CurrentLevel", 1);
            SceneManager.LoadScene($"Level {lvl}");
        }
        else Debug.LogError("No saved game data found");
    }

    public void ContinueGameNo()
    {
        PlayClick();
        SaveLoadManager.DeleteSave();
        PickupItemManager.pickedUpItemIds.Clear();

        continueGameCanvas.SetActive(false);
        levelCanvas.SetActive(true);
    }

    public void LevelButton()
    {
        PlayClick();
        mainMenuCanvas.GetComponent<Canvas>().enabled = false;

        if ( SaveLoadManager.SaveExists() )
        {
            int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

            if ( continueGameText != null )
                continueGameText.text =
                    $"YOU HAVE A SAVED GAME FOR LEVEL {savedLevel}.\nDO YOU WISH TO CONTINUE?";

            Debug.Log("Saved game found");
            continueGameCanvas.SetActive(true);
        }
        else
        {
            levelCanvas.SetActive(true);
        }
    }

    void ShowPreviewBackground(bool showPreview)
    {
        if (backgroundImage  != null) backgroundImage .SetActive(!showPreview);
        if (backgroundPreview != null) backgroundPreview.SetActive(showPreview);
    }

    public void ScoreButton()
    {
        PlayClick();
        mainMenuCanvas.GetComponent<Canvas>().enabled = false;
        scoreCanvas.SetActive(true);

        selectedLevel = 1;

        RefreshScoreDisplay();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scoreLevel1TabButton.gameObject);
    }

    public void SettingsButton()
    {
        PlayClick();
        mainMenuCanvas.GetComponent<Canvas>().enabled = false;
        settingsCanvas.GetComponent<Canvas>().enabled = true;
        controlsCanvas.SetActive(true);
        audioCanvas.SetActive(false);
        brightnessCanvas.SetActive(false);
        ShowPreviewBackground(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsTabButton.gameObject);
    }

    public void ControlsButton()
    {
        PlayClick();
        controlsCanvas.SetActive(true);
        audioCanvas.SetActive(false);
        brightnessCanvas.SetActive(false);
        ShowPreviewBackground(false);
    }

    public void AudioButton()
    {
        PlayClick();
        controlsCanvas.SetActive(false);
        audioCanvas.SetActive(true);
        brightnessCanvas.SetActive(false);
        ShowPreviewBackground(false);
    }

    public void BrightnessButton()
    {
        PlayClick();
        controlsCanvas.SetActive(false);
        audioCanvas.SetActive(false);
        brightnessCanvas.SetActive(true);
        ShowPreviewBackground(true);
    }

    public void ReturnToMainMenuButton()
    {
        PlayClick();
        mainMenuCanvas.GetComponent<Canvas>().enabled = true;
        settingsCanvas.GetComponent<Canvas>().enabled = false;
        scoreCanvas.SetActive(false);
        levelCanvas.SetActive(false);
        continueGameCanvas.SetActive(false);
        ShowPreviewBackground(false);
    }

    public void ExitGameButton()
    {
        PlayClick();
        Application.Quit();
    }

    // high score display
    public void OnLevel1ButtonClicked() => OnLevelButtonClicked(1);
    public void OnLevel2ButtonClicked() => OnLevelButtonClicked(2);

    public void OnLevelButtonClicked(int level)
    {
        PlayClick();
        selectedLevel = level;
        RefreshScoreDisplay();
    }

    private void RefreshScoreDisplay()
    {
        StartCoroutine(
          RemoteHighScoreManager.Instance.GetHighScores(
            playerId, 
            selectedLevel, 
            rows => DisplayScores(rows)
          )
        );
    }

    private void DisplayScores(List<RemoteHighScoreManager.HighScoreRow> rows)
    {
        string left  = "";
        string right = "";

        for (int i = 0; i < rows.Count; i++)
        {
            string line = $"{i+1}. {rows[i].score} Coins\n";
            if (i < 5) left  += line;
            else right += line;
        }

        scoreTextLeft .text = left;
        scoreTextRight.text = right;
    }

    private void PlayClick()
    {
        if (buttonSound != null) buttonSound.Play();
    }
}

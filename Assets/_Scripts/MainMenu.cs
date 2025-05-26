using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject scoreMenu;
    public GameObject levelsMenu;
    public GameObject continueGameMenu;
    public TMP_Text continueGameText;

    public GameObject controlsMenu;
    public GameObject audioMenu;
    public GameObject brightnessMenu;

    public Button[] levelButtons;

    public TMP_Text scoreTextLeft;
    public TMP_Text scoreTextRight;
    public Button scoreLevel1TabButton;

    public GameObject backgroundImage;
    public GameObject backgroundPreview;
    public Button controlsTabButton;

    public AudioSource buttonSound;

    private string playerId;
    private int selectedLevel = 1;

    void Start()
    {
        AudioListener.pause = false;

        ShowMainMenu();

        playerId = PlayerIdManager.instance.GetPlayerId();
        LockLevelButtons();
        RefreshScoreDisplay();
    }

    void LockLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < levelButtons.Length; i++)
            levelButtons[i].interactable = (i + 1) <= unlockedLevel;
    }

    // buttons
    public void StartButton(string sceneName)
    {
        PlayClick();

        if (string.IsNullOrEmpty(sceneName)) return;

        var digits = new string(sceneName.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out int lvl))
            PlayerPrefs.SetInt("CurrentLevel", lvl);
        PlayerPrefs.Save();

        SceneManager.LoadScene(sceneName);
    }

    public void LevelButton()
    {
        PlayClick();
        mainMenu.SetActive(false);

        if (SaveLoadManager.SaveExists())
        {
            int saved = PlayerPrefs.GetInt("CurrentLevel", 1);
            continueGameText.text =
                $"YOU HAVE A SAVED GAME FOR LEVEL {saved}.\nDO YOU WISH TO CONTINUE?";
            continueGameMenu.SetActive(true);
        }
        else
        {
            levelsMenu.SetActive(true);
        }
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
        continueGameMenu.SetActive(false);
        levelsMenu.SetActive(true);
    }

    public void ScoreButton()
    {
        PlayClick();
        mainMenu.SetActive(false);
        scoreMenu.SetActive(true);

        selectedLevel = 1;
        RefreshScoreDisplay();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scoreLevel1TabButton.gameObject);
    }

    public void SettingsButton()
    {
        PlayClick();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        ShowControls();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsTabButton.gameObject);
    }

    public void ControlsButton()   { PlayClick(); ShowControls(); }
    public void AudioButton()      { PlayClick(); ShowAudio(); }
    public void BrightnessButton(){ PlayClick(); ShowBrightness(); }

    public void ReturnToMainMenuButton()
    {
        PlayClick();
        ShowMainMenu();
    }

    public void ExitGameButton()
    {
        PlayClick();
        Application.Quit();
    }

    // high score display
    public void OnLevel1ButtonClicked() => OnLevelButtonClicked(1);
    public void OnLevel2ButtonClicked() => OnLevelButtonClicked(2);

    void OnLevelButtonClicked(int level)
    {
        PlayClick();
        selectedLevel = level;
        RefreshScoreDisplay();
    }

    void RefreshScoreDisplay()
    {
        StartCoroutine(
            RemoteHighScoreManager.Instance.GetHighScores(
                playerId,
                selectedLevel,
                rows => DisplayScores(rows)
            )
        );
    }

    void DisplayScores(List<RemoteHighScoreManager.HighScoreRow> rows)
    {
        string left = "";
        string right = "";

        for (int i = 0; i < rows.Count; i++)
        {
            string line = $"{i+1}. {rows[i].score} Coins\n";
            if (i < 5) left += line;
            else right += line;
        }

        scoreTextLeft.text = left;
        scoreTextRight.text = right;
    }

    void ShowMainMenu()
    {
        mainMenu.SetActive(true);

        optionsMenu.SetActive(false);
        scoreMenu.SetActive(false);
        levelsMenu.SetActive(false);
        continueGameMenu.SetActive(false);

        controlsMenu.SetActive(false);
        audioMenu.SetActive(false);
        brightnessMenu.SetActive(false);

        ShowPreviewBackground(false);
    }

    void ShowControls()
    {
        controlsMenu.SetActive(true);
        audioMenu.SetActive(false);
        brightnessMenu.SetActive(false);
        ShowPreviewBackground(false);
    }

    void ShowAudio()
    {
        controlsMenu.SetActive(false);
        audioMenu.SetActive(true);
        brightnessMenu.SetActive(false);
        ShowPreviewBackground(false);
    }

    void ShowBrightness()
    {
        controlsMenu.SetActive(false);
        audioMenu.SetActive(false);
        brightnessMenu.SetActive(true);
        ShowPreviewBackground(true);
    }

    void ShowPreviewBackground(bool show)
    {
        if (backgroundImage != null)  backgroundImage.SetActive(!show);
        if (backgroundPreview != null) backgroundPreview.SetActive(show);
    }

    void PlayClick()
    {
        if (buttonSound != null) buttonSound.Play();
    }
}

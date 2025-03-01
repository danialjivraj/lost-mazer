using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using BackendlessAPI;
using BackendlessAPI.Persistence;
using System.Threading.Tasks;
using System.Linq;

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
    public int selectedLevel = 1;

    public GameObject continueGameCanvas;
    public TMP_Text continueGameText;

    void Start()
    {
        UpdateScoreDisplay();

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
        continueGameCanvas.SetActive(false);
        
        if (SaveLoadManager.SaveExists())
        {
            int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            if (continueGameText != null)
            {
                continueGameText.text = "YOU HAVE A SAVED GAME FOR LEVEL " + savedLevel + ".\nDO YOU WISH TO CONTINUE?";
            }
            Debug.Log("Saved game found. ContinueGameCanvas will be used.");
        }

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

        if (SaveLoadManager.SaveExists())
        {
            continueGameCanvas.SetActive(true);
            return;
        }

        if (!string.IsNullOrEmpty(selectedSceneName))
        {
            var digits = new string(selectedSceneName.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out int levelNumber))
            {
                PlayerPrefs.SetInt("CurrentLevel", levelNumber);
            }
            else
            {
                Debug.LogWarning("Could not determine level number from scene name: " + selectedSceneName);
            }
            
            PlayerPrefs.Save();
            SceneManager.LoadScene(selectedSceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty!");
        }
    }

    public void ContinueGameYes()
    {
        buttonSound.Play();
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            Debug.LogError("No saved game data found!");
        }
    }

    public void ContinueGameNo()
    {
        buttonSound.Play();
        SaveLoadManager.DeleteSave(); // later on find a way to actually delete all this stuff as soon as player enters the game
        PickupItemManager.pickedUpItemIds.Clear(); // later on find a way to actually delete all this stuff as soon as player enters the game
        continueGameCanvas.SetActive(false);
        level.SetActive(true);
    }

    public void LevelButton()
    {
        buttonSound.Play();
        mainMenu.GetComponent<Canvas>().enabled = false;
        
        if (SaveLoadManager.SaveExists())
        {
            level.SetActive(false);
            controls.SetActive(false);
            score.SetActive(false);
            Audio.SetActive(false);
            continueGameCanvas.SetActive(true);
        }
        else
        {
            continueGameCanvas.SetActive(false);
            level.SetActive(true);
            controls.SetActive(false);
            score.SetActive(false);
            Audio.SetActive(true);
        }
    }

    public void ScoreButton()
    {
        buttonSound.Play();

        mainMenu.GetComponent<Canvas>().enabled = false;
        score.SetActive(true);
        controls.SetActive(false);
        level.SetActive(false);
        Audio.SetActive(true);
    }

    async void UpdateScoreDisplay()
    {
        try
        {
            Debug.Log("Updating score display...");

            if (scoreTextLeft == null)
                Debug.LogError("scoreTextLeft is null!");
            if (scoreTextRight == null)
                Debug.LogError("scoreTextRight is null!");

            string playerId = PlayerIdManager.instance.GetPlayerId();

            var dataStore = Backendless.Data.Of("HighScores");
            var query = DataQueryBuilder.Create();
            query.SetWhereClause($"playerId = '{playerId}' AND level = {selectedLevel}");
            query.SetPageSize(10).SetSortBy(new List<string> { "score DESC" });

            Debug.Log("Fetching scores from Backendless...");
            var result = await dataStore.FindAsync(query);

            if (result == null)
            {
                Debug.LogError("Result is null!");
                return;
            }

            Debug.Log($"Found {result.Count} scores for player {playerId}.");

            string leftScores = "";
            string rightScores = "";

            for (int i = 0; i < result.Count; i++)
            {
                if (i < 5)
                {
                    leftScores += (i + 1) + ". " + result[i]["score"] + " Coins\n";
                }
                else
                {
                    rightScores += (i + 1) + ". " + result[i]["score"] + " Coins\n";
                }
            }

            scoreTextLeft.text = leftScores;
            scoreTextRight.text = rightScores;

            Debug.Log("Score display updated successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error updating score display: " + ex.Message);
        }
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
        continueGameCanvas.SetActive(false);
    }

    public async void OnLevelButtonClicked(int level)
    {
        buttonSound.Play();

        string playerId = PlayerIdManager.instance.GetPlayerId();
        var dataStore = Backendless.Data.Of("HighScores");
        var query = DataQueryBuilder.Create();
        query.SetWhereClause($"playerId = '{playerId}' AND level = {level}");
        query.SetPageSize(10).SetSortBy(new List<string> { "score DESC" });

        var result = await dataStore.FindAsync(query);

        string leftScores = "";
        string rightScores = "";

        for (int i = 0; i < result.Count; i++)
        {
            string scoreLine = (i + 1) + ". " + result[i]["score"] + " Coins\n";
            if (i < 5)
                leftScores += scoreLine;
            else
                rightScores += scoreLine;
        }

        scoreTextLeft.text = leftScores;
        scoreTextRight.text = rightScores;
    }

    public void OnLevel1ButtonClicked()
    {
        OnLevelButtonClicked(1);
    }

    public void OnLevel2ButtonClicked()
    {
        OnLevelButtonClicked(2);
    }
}

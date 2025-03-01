using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject audioSettingsMenu;
    public GameObject buttons;
    public GameObject crosshair;
    public bool isPaused;
    public MonoBehaviour player;

    private bool isDragging = false;
    private Vector3 dragOffset;

    void Start()
    {
        pauseMenu.SetActive(false);
        audioSettingsMenu.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (audioSettingsMenu.activeSelf)
                {
                    CloseAudioSettings();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }

        if (isDragging && crosshair != null)
        {
            Vector3 mousePosition = Input.mousePosition + dragOffset;
            crosshair.transform.position = mousePosition;

            if (Vector3.Distance(crosshair.transform.position, new Vector3(Screen.width / 2, Screen.height / 2, 0)) > 100f)
            {
                crosshair.SetActive(false);
                isDragging = false;
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Pause();
        }

        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshair != null) crosshair.SetActive(false);

        if (player != null)
        {
            player.enabled = false;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        audioSettingsMenu.SetActive(false);
        Time.timeScale = 1f;

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.UnPause();
        }

        isPaused = false;

        ReadNotes noteScript = FindObjectOfType<ReadNotes>();
        if (noteScript != null && noteScript.noteUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (player != null) player.enabled = false;
        }
        else
        {
            LockCursor();
            if (player != null) player.enabled = true;
        }

        if (crosshair != null && !isDragging)
        {
            crosshair.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        //SaveLoadManager.DeleteSave();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnCrosshairDragStart()
    {
        if (!isPaused) return;

        isDragging = true;
        dragOffset = crosshair.transform.position - Input.mousePosition;
    }

    public void OnCrosshairDragEnd()
    {
        isDragging = false;
    }

    public void OpenAudioSettings()
    {
        Debug.Log("Opening Audio Settings");
        buttons.SetActive(false);
        audioSettingsMenu.SetActive(true);
    }

    public void CloseAudioSettings()
    {
        Debug.Log("Closing Audio Settings");
        audioSettingsMenu.SetActive(false);
        buttons.SetActive(true);
    }

    public void SaveAndGoBackToMainMenu()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            GameStateData data = new GameStateData();
            data.playerPosition = playerController.transform.position;
            data.playerRotation = playerController.transform.rotation;
            SaveLoadManager.SaveGame(data);
            Debug.Log("Game state saved. Returning to main menu.");
        }
        else
        {
            Debug.LogError("PlayerController not found!");
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}

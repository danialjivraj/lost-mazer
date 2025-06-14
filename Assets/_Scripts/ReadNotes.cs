using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadNotes : MonoBehaviour
{
    public string noteId;
    
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;
    public HandUIHandler handUIHandler;
    public GameObject readableNoteUI;

    public AudioSource pickUpSound;
    public AudioSource closeSound;

    private bool inReach;
    private PlayerController playerController;

    private bool isReadableViewActive = false;
    public static bool IsReadingNote { get; private set; } = false;
    public List<SubtitleData> subtitles;
    [SerializeField] private bool isPasswordNote = false;

    void Awake()
    {
        if (string.IsNullOrEmpty(noteId))
        {
            noteId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;
        }
    }

    void Start()
    {
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
        else
            Debug.LogError("Player GameObject is not assigned in ReadNotes");

        if (noteUI != null) noteUI.SetActive(false);
        if (hud != null) hud.SetActive(true);
        if (inv != null) inv.SetActive(true);
        if (readableNoteUI != null) readableNoteUI.SetActive(false);
        inReach = false;

        GameStateData loadedData = SaveLoadManager.LoadGame();
        if (loadedData != null && loadedData.isReadingNoteActive && loadedData.activeNoteId == this.noteId)
        {
            OpenNote();
            if (loadedData.isReadableViewActive)
            {
                ToggleReadableNote();
            }
        }
    }

    IEnumerator DisablePlayerControllerNextFrame()
    {
        yield return null;
        if (playerController != null)
        {
            playerController.enabled = false;

            playerController.StopAllMovementAudio();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (handUIHandler != null) handUIHandler.ShowHandUI();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (handUIHandler != null) handUIHandler.HideHandUI();
        }
    }

    void Update()
    {
        if (handUIHandler != null && handUIHandler.IsGamePaused())
            return;

        if (Time.timeScale != 0)
        {
            if (inReach && !noteUI.activeSelf && Input.GetButtonDown("Interact"))
            {
                OpenNote();
            }

            if (noteUI.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                ToggleReadableNote();
            }
        }
    }

    void OpenNote()
    {
        SubtitleManager.Instance.ResetSubtitles();

        if (noteUI != null) noteUI.SetActive(true);
        if (pickUpSound != null) pickUpSound.Play();
        if (hud != null) hud.SetActive(false);
        if (inv != null) inv.SetActive(false);
        if (handUIHandler != null) handUIHandler.HideHandUI();

        if (playerController != null)
        {
            StartCoroutine(DisablePlayerControllerNextFrame()); // this prevents the player from getting into the default position when reloading on an active note
        }
        
        IsReadingNote = true;
    }

    void ToggleReadableNote()
    {
        if (readableNoteUI != null)
        {
            isReadableViewActive = !isReadableViewActive;
            readableNoteUI.SetActive(isReadableViewActive);
        }
    }

    public void ExitButton()
    {
        if (closeSound != null)
            closeSound.Play();

        if (noteUI != null) noteUI.SetActive(false);
        if (readableNoteUI != null) readableNoteUI.SetActive(false);

        if (hud != null) hud.SetActive(true);
        if (inv != null) inv.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isReadableViewActive = false;
        IsReadingNote = false;

        SubtitleManager.Instance.ResetSubtitles();

        if (isPasswordNote)
        {
            int pw = PasswordManager.CurrentPassword;
            string password = pw.ToString("D4");  
            SubtitleData benLine = new SubtitleData {
                spawnDelay = 0.25f,
                text = $"Ben: {password}? Better not forget this number...",
                displayTime = 3f,
            };
            SubtitleManager.Instance.EnqueueSubtitle(benLine);
        }

        foreach (SubtitleData subtitle in subtitles)
            SubtitleManager.Instance.EnqueueSubtitle(subtitle);

        isReadableViewActive = false;
        IsReadingNote = false;
    }


    public bool GetIsReadableViewActive()
    {
        return isReadableViewActive;
    }
}

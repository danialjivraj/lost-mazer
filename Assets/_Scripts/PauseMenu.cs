using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject audioSettingsMenu;
    public GameObject buttons;
    public GameObject crosshair;
    public bool isPaused;
    public MonoBehaviour player;
    public AudioSource buttonSound;
    public Button saveAndGoBackButton;

    void Start()
    {
        pauseMenu.SetActive(false);
        audioSettingsMenu.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
        LockCursor();
        buttonSound.ignoreListenerPause = true;
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
    }

    public void PauseGame()
    {
        buttonSound.Play();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
           if (audio != buttonSound)
               audio.Pause();
        }

        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshair != null) crosshair.SetActive(false);
        if (player != null) player.enabled = false;

        if (TriggerCutscene.isCutsceneActive)
        {
            saveAndGoBackButton.interactable = false;
        }
        else
        {
            saveAndGoBackButton.interactable = true;
        }
    }

    public void ResumeGame()
    {
        buttonSound.Play();
        pauseMenu.SetActive(false);
        audioSettingsMenu.SetActive(false);
        Time.timeScale = 1f;

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.UnPause();
        }

        isPaused = false;

        if (ReadNotes.IsReadingNote)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (player != null)
                player.enabled = false;
        }
        else
        {
            if (!TriggerCutscene.isCutsceneActive)
            {
                LockCursor();
                if (player != null)
                    player.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        AudioListener.pause = true;
        buttonSound.Play();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private IEnumerator RetryAfterSound()
    {
        yield return new WaitForSecondsRealtime(buttonSound.clip.length);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Retry()
    {
        buttonSound.Play();
        StartCoroutine(RetryAfterSound());
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenAudioSettings()
    {
        buttonSound.Play();
        Debug.Log("Opening Audio Settings");
        buttons.SetActive(false);
        audioSettingsMenu.SetActive(true);
    }

    public void CloseAudioSettings()
    {
        buttonSound.Play();
        Debug.Log("Closing Audio Settings");
        audioSettingsMenu.SetActive(false);
        buttons.SetActive(true);
    }

    public void SaveAndGoBackToMainMenu()
    {
        AudioListener.pause = true;
        buttonSound.Play();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        PlayerHealth playerHealthScript = FindObjectOfType<PlayerHealth>();
        
        ReadNotes[] allNotes = FindObjectsOfType<ReadNotes>();
        bool noteFound = false;

        FinalDoor finalDoor = FindObjectOfType<FinalDoor>();
        RagdollController[] allRagdolls = FindObjectsOfType<RagdollController>();
        NavKeypad.Keypad keypad = FindObjectOfType<NavKeypad.Keypad>();
        NavKeypad.SlidingDoor slidingDoor = FindObjectOfType<NavKeypad.SlidingDoor>();
        TriggerCutscene[] cutscenes = FindObjectsOfType<TriggerCutscene>();

        if (playerController != null && playerHealthScript != null)
        {
            GameStateData data = new GameStateData();
            // player-related state
            data.playerPosition = playerController.transform.position;
            data.playerRotation = playerController.transform.rotation;
            data.cameraRotation = playerController.playerCam.transform.localRotation;
            data.rotationX = playerController.RotationX;
            data.rotationY = playerController.RotationY;
            data.isCrouching = playerController.IsCrouching;
            data.isZoomed = false;
            data.currentHeight = playerController.CurrentHeight;
            data.cameraFOV = playerController.playerCam.fieldOfView;
            data.score = ScoreManager.instance.CurrentScore;
            data.playerHealth = playerHealthScript.CurrentHealth;

            // key
            if (KeyController.instance != null)
            {
                data.playerHasKey = KeyController.instance.hasKey;
            }

            // reading note
            foreach (ReadNotes note in allNotes)
            {
                if (note.noteUI.activeSelf) // the note is currently open
                {
                    data.activeNoteId = note.noteId;
                    data.isReadingNoteActive = true;
                    data.isReadableViewActive = note.GetIsReadableViewActive();
                    noteFound = true;
                    break; // only one note can be active at a time
                }
            }

            if (!noteFound)
            {
                data.activeNoteId = "";
                data.isReadingNoteActive = false;
                data.isReadableViewActive = false;
            }

            // keypad
            if(keypad != null)
            {
                data.keypadCurrentInput = keypad.GetCurrentInput();
                data.keypadDeniedState = keypad.GetDeniedState();
                data.keypadAccessWasGranted = keypad.GetAccessWasGranted();
            }
            data.doorPassword = PasswordManager.CurrentPassword; // password
            // sliding door
            if(slidingDoor != null)
            {
                data.slidingDoorIsOpen = slidingDoor.IsOpoen;
                if(data.slidingDoorIsOpen)
                {
                    data.slidingDoorAnimTime = slidingDoor.GetAnimNormalizedTime();
                }
                else
                {
                    data.slidingDoorAnimTime = 0f;
                }
            }

            // chest states
            UseChest[] chests = FindObjectsOfType<UseChest>();
            foreach (UseChest chest in chests)
            {
                ChestState chestState = new ChestState
                {
                    chestId = chest.chestId,
                    isOpen = chest.GetChestState(),
                    itemPickedUpStates = new List<bool>()
                };

                for (int i = 0; i < chest.objectsToActivate.Length; i++)
                {
                    bool pickedUp = chest.objectsToActivate[i] == null;
                    chestState.itemPickedUpStates.Add(pickedUp);
                }

                data.chestStates.Add(chestState);
            }

            // pickup item states
            List<PickupItemState> pickupStates = new List<PickupItemState>();
            // respawnable items
            PickUpItems[] respawnPickups = Resources.FindObjectsOfTypeAll<PickUpItems>();
            foreach (PickUpItems pickup in respawnPickups)
            {
                if (pickup.shouldRespawn)
                {
                    PickupItemState state = new PickupItemState
                    {
                        itemId = pickup.itemId,
                        isPickedUp = PickupItemManager.pickedUpItemIds.Contains(pickup.itemId),
                        remainingRespawnTime = (!pickup.gameObject.activeInHierarchy) ? 
                            RespawnManager.Instance.GetRemainingRespawnTime(pickup.itemId) : 0
                    };
                    pickupStates.Add(state);
                }
                else
                {
                    // non-respawnable items found in the scene that have NOT been picked up
                    PickupItemState state = new PickupItemState
                    {
                        itemId = pickup.itemId,
                        isPickedUp = false,
                        remainingRespawnTime = 0
                    };
                    pickupStates.Add(state);
                }
            }
            // non-respawnable items found in the scene that HAVE been picked up
            foreach (string id in PickupItemManager.pickedUpItemIds)
            {
                if (!pickupStates.Exists(s => s.itemId == id))
                {
                    PickupItemState state = new PickupItemState
                    {
                        itemId = id,
                        isPickedUp = true,
                        remainingRespawnTime = 0
                    };
                    pickupStates.Add(state);
                }
            }
            data.pickupItemStates = pickupStates;

            // lantern
            LanternController lanternController = FindObjectOfType<LanternController>();
            if (lanternController != null)
            {
                data.lanternState = new LanternState();
                data.lanternState.hasLantern = lanternController.HasLantern();
                data.lanternState.isLanternActive = lanternController.IsLanternActive();
            }

            // locker
            foreach (LockerInteraction locker in LockerInteraction.allLockers)
            {
                data.lockerStates.Add(locker.GetLockerState());
            }

            // enemy
            EnemyController enemy = FindObjectOfType<EnemyController>();
            if (enemy != null)
            {
                data.enemyStates.Add(enemy.GetEnemyState());
            }

            if (finalDoor != null)
            {
                data.finalDoorIsUnlocked = !finalDoor.IsLocked();

                if(data.finalDoorIsUnlocked)
                {
                    data.finalDoorAnimationTime = finalDoor.doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                }
                else
                {
                    data.finalDoorAnimationTime = 0f;
                }
            }

            // triggers
            Trigger[] triggers = FindObjectsOfType<Trigger>();
            foreach (Trigger trigger in triggers)
            {
                TriggerState tState = new TriggerState
                {
                    triggerId = trigger.triggerId,
                    hasTriggered = trigger.GetTriggeredState()
                };
                data.triggerStates.Add(tState);
            }

            // ragdoll
            foreach (RagdollController ragdoll in allRagdolls)
            {
                RagdollState rState = ragdoll.SaveRagdollState();
                data.ragdollStates.Add(rState);
            }

            // cutscenes
            foreach (TriggerCutscene cs in cutscenes)
            {
                data.cutsceneStates.RemoveAll(c => c.cutsceneId == cs.cutsceneId);
                data.cutsceneStates.Add(new CutsceneState { cutsceneId = cs.cutsceneId, hasPlayed = cs.hasPlayed });
            }

            // ensuring the right level is saved
            string sceneName = SceneManager.GetActiveScene().name;
            int levelNum = int.Parse( new string(sceneName.Where(char.IsDigit).ToArray()) );
            PlayerPrefs.SetInt("CurrentLevel", levelNum);
            PlayerPrefs.Save();

            SaveLoadManager.SaveGame(data);
            Debug.Log("Game state saved, returning to main menu");
        }
        else
        {
            Debug.LogError("PlayerController or PlayerHealth not found");
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}

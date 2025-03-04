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
    public AudioSource buttonSound;

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
        buttonSound.Play();
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
        buttonSound.Play();
        //SaveLoadManager.DeleteSave();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private IEnumerator RetryAfterSound()
    {
        yield return new WaitForSeconds(buttonSound.clip.length);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Retry()
    {
        buttonSound.Play();
        Time.timeScale = 1f;
        StartCoroutine(RetryAfterSound());
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
        buttonSound.Play();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        PlayerHealth playerHealthScript = FindObjectOfType<PlayerHealth>();
        ReadNotes readNotes = FindObjectOfType<ReadNotes>();
        RagdollController[] allRagdolls = FindObjectsOfType<RagdollController>();
        NavKeypad.Keypad keypad = FindObjectOfType<NavKeypad.Keypad>();
        NavKeypad.SlidingDoor slidingDoor = FindObjectOfType<NavKeypad.SlidingDoor>();

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
            data.isReadingNote = ReadNotes.IsReadingNote;
            data.isReadableViewActive = readNotes.GetIsReadableViewActive();

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

            foreach (RagdollController ragdoll in allRagdolls)
            {
                RagdollState rState = ragdoll.SaveRagdollState();
                data.ragdollStates.Add(rState);
            }

            SaveLoadManager.SaveGame(data);
            Debug.Log("Game state saved. Returning to main menu.");
        }
        else
        {
            Debug.LogError("PlayerController or PlayerHealth not found!");
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}

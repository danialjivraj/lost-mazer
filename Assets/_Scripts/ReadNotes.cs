using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadNotes : MonoBehaviour
{
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;
    public GameObject handUI;
    public GameObject readableNoteUI;

    public AudioSource pickUpSound;

    private bool inReach;
    private PlayerController playerController;

    private bool isReadableViewActive = false;
    public static bool IsReadingNote { get; private set; } = false;

    void Start()
    {
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
        else
            Debug.LogError("Player GameObject is not assigned in ReadNotes!");

        if (noteUI != null) noteUI.SetActive(false);
        if (hud != null) hud.SetActive(true);
        if (inv != null) inv.SetActive(true);
        if (handUI != null) handUI.SetActive(false);
        if (readableNoteUI != null) readableNoteUI.SetActive(false);

        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (handUI != null) handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (handUI != null) handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (inReach && Input.GetButtonDown("Interact"))
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
        if (noteUI != null) noteUI.SetActive(true);
        if (pickUpSound != null) pickUpSound.Play();
        if (hud != null) hud.SetActive(false);
        if (inv != null) inv.SetActive(false);
        if (handUI != null) handUI.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
    }
}

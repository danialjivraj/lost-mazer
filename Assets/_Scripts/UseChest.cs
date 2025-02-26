using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public HandUIHandler handUIHandler;
    public GameObject[] objectsToActivate;

    public AudioSource chestOpenSound;
    public float soundVolume = 1.0f;

    private bool inReach;
    private bool isChestOpen = false;
    public PickUpNotification pickUpNotification;

    void Start()
    {
        OB = this.gameObject;

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }

        if (chestOpenSound != null)
        {
            chestOpenSound.volume = soundVolume;
        }
        else
        {
            Debug.LogWarning("Chest open sound AudioSource is not assigned.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !isChestOpen)
        {
            inReach = true;
            if (handUIHandler != null)
                handUIHandler.ShowHandUI();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (handUIHandler != null)
                handUIHandler.HideHandUI();
        }
    }

    void Update()
    {
        if (handUIHandler != null && handUIHandler.IsGamePaused())
            return;

        if (inReach && Input.GetButtonDown("Interact") && !isChestOpen)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (chestOpenSound != null && !chestOpenSound.isPlaying)
        {
            chestOpenSound.Play();
        }
        else
        {
            Debug.LogWarning("Chest open sound AudioSource is not assigned or is already playing.");
        }

        if (handUIHandler != null)
            handUIHandler.HideHandUI();

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        if (OB.GetComponent<Animator>() != null)
        {
            OB.GetComponent<Animator>().SetBool("open", true);
        }

        if (OB.GetComponent<BoxCollider>() != null)
        {
            OB.GetComponent<BoxCollider>().enabled = false;
        }

        isChestOpen = true;

        if (pickUpNotification != null)
        {
            pickUpNotification.ShowNotification();
        }
    }
}
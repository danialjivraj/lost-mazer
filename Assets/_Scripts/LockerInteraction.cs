using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    private GameObject locker;
    public HandUIHandler handUIHandler;
    public Animator lockerAnimator;
    public Transform player;

    private bool inReach;
    private bool isLockerOpen = false;
    private bool isPlayerInside = false;

    public AudioSource lockerOpenSound;
    public AudioSource lockerCloseSound;
    public float soundVolume = 1.0f;
    public LockerTrigger lockerTrigger;

    void Start()
    {
        locker = this.gameObject;

        if (lockerOpenSound != null)
            lockerOpenSound.volume = soundVolume;

        if (lockerCloseSound != null)
            lockerCloseSound.volume = soundVolume;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (handUIHandler != null)
                handUIHandler.ShowHandUI();
        }

        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
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

        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    void Update()
    {
        if (handUIHandler != null && handUIHandler.IsGamePaused())
            return;

        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleLocker();
        }
    }

    void ToggleLocker()
    {
        isLockerOpen = !isLockerOpen;

        if (isLockerOpen && lockerOpenSound != null && !lockerOpenSound.isPlaying)
        {
            lockerOpenSound.Play();
        }
        else if (!isLockerOpen && lockerCloseSound != null && !lockerCloseSound.isPlaying)
        {
            lockerCloseSound.Play();
        }

        if (lockerAnimator != null)
        {
            lockerAnimator.SetBool("isOpen", isLockerOpen);
        }
    }

    public bool IsPlayerHiding()
    {
        return lockerTrigger != null && lockerTrigger.isPlayerInside && !isLockerOpen;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    // Static list of all active LockerInteraction instances.
    private static List<LockerInteraction> allLockers = new List<LockerInteraction>();

    // Existing fields...
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

    void Awake()
    {
        // Add this instance to the static list.
        allLockers.Add(this);

        // Set up audio volumes.
        if (lockerOpenSound != null)
            lockerOpenSound.volume = soundVolume;
        if (lockerCloseSound != null)
            lockerCloseSound.volume = soundVolume;
    }

    void OnDestroy()
    {
        // Remove this instance when it's destroyed.
        allLockers.Remove(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
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

        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleLocker();
        }
    }

    public void ToggleLocker()
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

    public static bool IsAnyLockerHiding()
    {
        foreach (var locker in allLockers)
        {
            if (locker.IsPlayerHiding())
                return true;
        }
        return false;
    }
}

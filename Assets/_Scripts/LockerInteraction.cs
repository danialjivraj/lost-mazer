using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    private GameObject locker;
    public GameObject handUI;
    public Animator lockerAnimator;
    public Transform player;

    private bool inReach;
    private bool isLockerOpen = false;
    private bool isPlayerInside = false;

    public AudioClip lockerOpenSound;
    public AudioClip lockerCloseSound;
    public float soundVolume = 1.0f;
    public LockerTrigger lockerTrigger;

    void Start()
    {
        locker = this.gameObject;

        if (handUI != null)
            handUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (handUI != null)
                handUI.SetActive(true);
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
            if (handUI != null)
                handUI.SetActive(false);
        }

        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleLocker();
        }
    }

    void ToggleLocker()
    {
        isLockerOpen = !isLockerOpen;

        AudioClip clip = isLockerOpen ? lockerOpenSound : lockerCloseSound;
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);

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
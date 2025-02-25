using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject[] objectsToActivate;

    public AudioClip chestOpenSound;
    public float soundVolume = 1.0f;

    private bool inReach;
    private bool isChestOpen = false;
    public PickUpNotification pickUpNotification;

    void Start()
    {
        OB = this.gameObject;

        if (handUI != null)
            handUI.SetActive(false);

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !isChestOpen)
        {
            inReach = true;
            if (handUI != null)
                handUI.SetActive(true);
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
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact") && !isChestOpen)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (chestOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(chestOpenSound, transform.position, soundVolume);
        }

        if (handUI != null)
            handUI.SetActive(false);

        // activate all objects in the array
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        // animation
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
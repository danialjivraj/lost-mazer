using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    public string itemId; // Unique identifier for this pickup item.
    public HandUIHandler handUIHandler;
    public GameObject objToActivate;
    public bool destroyAfterPickup = true;
    public float destroyDelay = 0.01f;

    public AudioSource pickupSound;
    public float soundVolume = 1.0f;
    private bool playGlobalSound = true;
    public PickUpNotification pickUpNotification;

    private bool inReach;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            // Auto-generate a unique id using name and position.
            itemId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;
        }
    }

    void Start()
    {
        // Check if this item was already picked up (loaded state).
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            PickupItemState state = data.pickupItemStates.Find(s => s.itemId == itemId);
            if (state != null && state.isPickedUp)
            {
                // Item was picked up previously, so remove it.
                Destroy(gameObject);
                return;
            }
        }

        if (objToActivate != null)
            objToActivate.SetActive(false);

        if (pickupSound != null)
        {
            pickupSound.volume = soundVolume;
        }
        else
        {
            Debug.LogWarning("Pickup sound AudioSource is not assigned.");
        }
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
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        if (pickupSound != null && !pickupSound.isPlaying)
        {
            if (playGlobalSound)
            {
                GameObject globalAudioObject = new GameObject("GlobalAudio");
                AudioSource globalAudioSource = globalAudioObject.AddComponent<AudioSource>();
                globalAudioSource.clip = pickupSound.clip;
                globalAudioSource.volume = soundVolume;
                globalAudioSource.outputAudioMixerGroup = pickupSound.outputAudioMixerGroup;
                globalAudioSource.Play();
                Destroy(globalAudioObject, pickupSound.clip.length);
            }
            else
            {
                pickupSound.Play();
            }
        }
        else
        {
            Debug.LogWarning("Pickup sound AudioSource is not assigned or is already playing.");
        }

        if (handUIHandler != null)
            handUIHandler.HideHandUI();

        if (objToActivate != null)
            objToActivate.SetActive(true);

        // if item is lantern
        LanternController lanternController = FindObjectOfType<LanternController>();
        if (lanternController != null && objToActivate != null && objToActivate.CompareTag("Lantern"))
        {
            lanternController.PickUpLantern();
        }

        // if item is coin
        if (objToActivate.CompareTag("Coin"))
        {
            ScoreManager.instance.AddPoint();
        }

        // Record that this item was picked up.
        if (!PickupItemManager.pickedUpItemIds.Contains(itemId))
            PickupItemManager.pickedUpItemIds.Add(itemId);

        if (destroyAfterPickup)
        {
            StartCoroutine(DestroyItemAfterDelay(destroyDelay));
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        if (pickUpNotification != null)
        {
            pickUpNotification.ShowNotification();
        }
    }

    IEnumerator DestroyItemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

// Static manager for tracking pickup items.
public static class PickupItemManager
{
    public static List<string> pickedUpItemIds = new List<string>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    public string itemId;
    public HandUIHandler handUIHandler;
    public GameObject objToActivate;
    public bool destroyAfterPickup = true;
    public float destroyDelay = 0.01f;

    public bool shouldRespawn = false;
    public float respawnTime = 5.0f;

    public AudioSource pickupSound;
    public float soundVolume = 1.0f;
    private bool playGlobalSound = true;
    public PickUpNotification pickUpNotification;

    public bool playSubtitles = true;
    public List<SubtitleData> subtitles;

    private bool inReach;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;
        }
    }

    void Start()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            PickupItemState state = data.pickupItemStates.Find(s => s.itemId == itemId);
            if (state != null)
            {
                // destroys non-respawnable items when picked up
                if (state.isPickedUp && !shouldRespawn)
                {
                    Destroy(gameObject);
                    return;
                }
                // resumes the countdown for respawnable
                if (shouldRespawn && state.isPickedUp)
                {
                    gameObject.SetActive(false);
                    float delay = state.remainingRespawnTime;
                    RespawnManager.Instance.StartCoroutine(RespawnManager.Instance.RespawnItem(gameObject, delay, itemId));
                    return;
                }
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

    void OnEnable()
    {
        inReach = false;
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

        // if item is key
        if (objToActivate.CompareTag("Key"))
        {
            if (KeyController.instance != null)
            {
                KeyController.instance.PickUpKey();
            }
        }

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

        if (!PickupItemManager.pickedUpItemIds.Contains(itemId))
            PickupItemManager.pickedUpItemIds.Add(itemId);

        if (shouldRespawn)
        {
            gameObject.SetActive(false);
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.StartCoroutine(RespawnManager.Instance.RespawnItem(gameObject, respawnTime, itemId));
            }
            else
            {
                Debug.LogWarning("RespawnManager instance not found!");
            }
        }
        else if (destroyAfterPickup)
        {
            StartCoroutine(DestroyItemAfterDelay(destroyDelay));
        }
        else
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;
        }

        if (pickUpNotification != null)
            pickUpNotification.ShowNotification();

        if (playSubtitles && subtitles != null && subtitles.Count > 0)
        {
            SubtitleManager.Instance.ResetSubtitles();
            foreach (SubtitleData subtitle in subtitles)
            {
                SubtitleManager.Instance.EnqueueSubtitle(subtitle);
            }
        }
    }

    IEnumerator DestroyItemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

// static manager for tracking pickup items
public static class PickupItemManager
{
    public static List<string> pickedUpItemIds = new List<string>();
}

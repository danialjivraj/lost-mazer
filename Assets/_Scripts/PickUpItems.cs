using System.Collections;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    public GameObject handUI;
    public GameObject objToActivate;
    public bool destroyAfterPickup = true;
    public float destroyDelay = 0.01f;

    public AudioClip pickupSound;
    public float soundVolume = 1.0f;
    public bool playGlobalSound = true;
    public PickUpNotification pickUpNotification;

    private bool inReach;

    void Start()
    {
        if (handUI != null)
            handUI.SetActive(false);

        if (objToActivate != null)
            objToActivate.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
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
        if (inReach && Input.GetButtonDown("Interact"))
        {
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        if (pickupSound != null)
        {
            if (playGlobalSound)
            {
                AudioSource globalAudioSource = new GameObject("GlobalAudio").AddComponent<AudioSource>();
                globalAudioSource.clip = pickupSound;
                globalAudioSource.volume = soundVolume;
                globalAudioSource.Play();
                Destroy(globalAudioSource.gameObject, pickupSound.length);
            }
            else
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
            }
        }

        if (handUI != null)
            handUI.SetActive(false);

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
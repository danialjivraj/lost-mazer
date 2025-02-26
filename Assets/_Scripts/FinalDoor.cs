using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public HandUIHandler handUIHandler;
    public GameObject invKey;
    public Animator doorAnimator;
    public PickUpNotification doorNotification;
    public AudioSource lockedAudioSource;
    public AudioSource openingAudioSource;

    private bool inReach;
    private bool isLocked = true;
    private bool isLockedAnimationPlaying = false;

    void Start()
    {
        if (doorNotification != null) doorNotification.ResetNotification();
    }

    void Update()
    {
        if (handUIHandler != null && handUIHandler.IsGamePaused())
            return;

        if (inReach && Input.GetButtonDown("Interact"))
        {
            if (isLocked)
            {
                if (invKey != null && invKey.activeInHierarchy)
                {
                    UnlockDoor();
                    if (doorNotification != null)
                        doorNotification.ResetNotification();
                }
                else
                {
                    PlayLockedAnimation();
                    if (doorNotification != null)
                    {
                        doorNotification.ShowNotification();
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (handUIHandler != null && isLocked)
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

        if (doorNotification != null)
            doorNotification.ResetNotification();
    }

    private void UnlockDoor()
    {
        isLocked = false;
        doorAnimator.SetBool("IsLocked", false);
        doorAnimator.SetTrigger("Unlocked");

        if (openingAudioSource != null)
        {
            openingAudioSource.Play();
        }

        if (handUIHandler != null)
            handUIHandler.HideHandUI();

        Debug.Log("Door is unlocked");
    }

    private void PlayLockedAnimation()
    {
        if (!isLockedAnimationPlaying)
        {
            doorAnimator.SetTrigger("Locked");

            if (lockedAudioSource != null)
            {
                lockedAudioSource.Play();
            }

            isLockedAnimationPlaying = true;

            StartCoroutine(ResetLockedAnimationFlag());
        }

        Debug.Log("Door is locked");
    }

    private System.Collections.IEnumerator ResetLockedAnimationFlag()
    {
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        isLockedAnimationPlaying = false;
    }
}
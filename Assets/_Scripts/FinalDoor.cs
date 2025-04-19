using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public HandUIHandler handUIHandler;
    public GameObject invKey;
    public Animator doorAnimator;
    public PickUpNotification doorNotification;
    public AudioSource lockedAudioSource;
    public AudioSource openingAudioSource;

    [SerializeField] private List<SubtitleData> lockSubtitles;
    [SerializeField] private List<SubtitleData> unlockSubtitles;

    private bool inReach;
    private bool isLocked = true;
    private bool isLockedAnimationPlaying = false;

    void Start()
    {
        if (SaveLoadManager.SaveExists())
        {
            GameStateData data = SaveLoadManager.LoadGame();
            if (data.finalDoorIsUnlocked)
            {
                isLocked = false;
                doorAnimator.SetBool("IsLocked", false);
                doorAnimator.Play("FinalDoor_Unlocked", 0, data.finalDoorAnimationTime);
            }
        }
        doorNotification?.ResetNotification();
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
                    doorNotification?.ResetNotification();
                }
                else
                {
                    PlayLockedAnimation();
                    doorNotification?.ShowNotification();
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
            handUIHandler?.HideHandUI();
        }
        doorNotification?.ResetNotification();
    }

    private void UnlockDoor()
    {
        isLocked = false;
        doorAnimator.SetBool("IsLocked", false);
        doorAnimator.SetTrigger("Unlocked");
        openingAudioSource?.Play();
        handUIHandler?.HideHandUI();
        Debug.Log("Door is unlocked");

        if (unlockSubtitles != null && unlockSubtitles.Count > 0)
        {
            SubtitleManager.Instance.ResetSubtitles();
            foreach (var sub in unlockSubtitles)
                SubtitleManager.Instance.EnqueueSubtitle(sub);
        }
    }

    private void PlayLockedAnimation()
    {
        if (!isLockedAnimationPlaying)
        {
            doorAnimator.SetTrigger("Locked");
            lockedAudioSource?.Play();
            isLockedAnimationPlaying = true;
            StartCoroutine(ResetLockedAnimationFlag());
        }
        Debug.Log("Door is locked");

        if (lockSubtitles != null && lockSubtitles.Count > 0)
        {
            SubtitleManager.Instance.ResetSubtitles();
            foreach (var sub in lockSubtitles)
                SubtitleManager.Instance.EnqueueSubtitle(sub);
        }
    }

    private IEnumerator ResetLockedAnimationFlag()
    {
        yield return new WaitForSeconds(
            doorAnimator.GetCurrentAnimatorStateInfo(0).length
        );
        isLockedAnimationPlaying = false;
    }

    public bool IsLocked() => isLocked;
}

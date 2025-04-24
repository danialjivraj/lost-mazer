using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    public static List<LockerInteraction> allLockers = new List<LockerInteraction>();

    public string lockerId;

    public HandUIHandler handUIHandler;
    public Animator lockerAnimator;
    public Transform player;
    private bool inReach;
    private bool isLockerOpen = false;
    public AudioSource lockerOpenSound;
    public AudioSource lockerCloseSound;
    public float soundVolume = 1.0f;
    public LockerTrigger lockerTrigger;

    void Awake()
    {
        if (string.IsNullOrEmpty(lockerId))
        {
            lockerId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;
        }

        allLockers.Add(this);

        if (lockerOpenSound != null)
            lockerOpenSound.volume = soundVolume;
        if (lockerCloseSound != null)
            lockerCloseSound.volume = soundVolume;
    }

    void Start()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            foreach (LockerState state in data.lockerStates)
            {
                LockerInteraction locker = LockerInteraction.allLockers.Find(l => l.lockerId == state.lockerId);
                if (locker != null)
                {
                    locker.ApplyLockerState(state);
                }
            }
        }
    }

    void OnDestroy()
    {
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

    public LockerState GetLockerState()
    {
        LockerState state = new LockerState
        {
            lockerId = lockerId,
            isOpen = isLockerOpen,
            isPlayerInside = lockerTrigger != null ? lockerTrigger.isPlayerInside : false
        };

        if (lockerAnimator != null)
        {
            AnimatorStateInfo stateInfo = lockerAnimator.GetCurrentAnimatorStateInfo(0);
            state.animNormalizedTime = stateInfo.normalizedTime;
        }

        return state;
    }

    public void ApplyLockerState(LockerState state)
    {
        isLockerOpen = state.isOpen;

        if (lockerAnimator != null)
        {
            string clipName = isLockerOpen ? "Locker_Open" : "Locker_Closed";
            
            lockerAnimator.Play(clipName, 0, state.animNormalizedTime);
            
            lockerAnimator.SetBool("isOpen", isLockerOpen);
        }
    }

    public bool IsPlayerHiding()
    {   
        return lockerTrigger != null && lockerTrigger.isPlayerInside && !isLockerOpen;
    }

    public static bool IsPlayerHidingInAnyLocker()
    {
        foreach (var locker in allLockers)
        {
            if (locker.IsPlayerHiding())
                return true;
        }
        return false;
    }
}

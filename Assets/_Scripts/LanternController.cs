using UnityEngine;
using System.Collections;

public class LanternController : MonoBehaviour
{
    private Animator animator;
    private bool isLanternActive = true;
    public GameObject arm;
    private bool hasLantern = false;
    private bool isAnimating = false;
    private LockerInteraction[] lockerInteractions;

    public PickUpNotification lanternWarning;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (arm == null)
        {
            arm = GameObject.Find("Lantern Arm");
            if (arm == null)
            {
                Debug.LogError("Lantern Arm NOT found in Awake!");
            }
        }

        if (animator == null)
        {
            Debug.LogError("Animator NOT found in Awake!");
        }

        lockerInteractions = FindObjectsOfType<LockerInteraction>();
        if (lockerInteractions.Length == 0)
        {
            Debug.LogError("🚨 No LockerInteraction scripts found!");
        }
    }

    void Start()
    {
        if (arm != null)
            arm.SetActive(false);

        isLanternActive = false;
        if (animator != null)
            animator.SetBool("isLanternActive", false);

        if (lanternWarning != null)
            lanternWarning.ResetNotification();

        // Attempt to load the lantern state from saved data.
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null && data.lanternState != null)
        {
            hasLantern = data.lanternState.hasLantern;
            isLanternActive = data.lanternState.isLanternActive;
            
            // If the player has the lantern, update the visuals accordingly.
            if (hasLantern)
            {
                if (arm != null)
                    arm.SetActive(isLanternActive);
                if (animator != null)
                    animator.SetBool("isLanternActive", isLanternActive);
            }
        }
    }

    void Update()
    {
        // toggles lantern if player presses X and has the lantern, is not animating, not reading a note, and not hiding
        if (Input.GetKeyDown(KeyCode.X) && hasLantern && !isAnimating && 
            !ReadNotes.IsReadingNote && !LockerInteraction.IsPlayerHidingInAnyLocker())
        {
            ToggleLantern();
        }

        // if the player is hiding and the lantern is active, turn it off
        if (LockerInteraction.IsPlayerHidingInAnyLocker() && isLanternActive)
        {
            ToggleLantern();
        }

        // shows warning if player presses X while hiding AND has the lantern
        if (Input.GetKeyDown(KeyCode.X) && LockerInteraction.IsPlayerHidingInAnyLocker() && hasLantern)
        {
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        if (lanternWarning != null)
        {
            lanternWarning.ShowNotification();
        }
    }

    void ToggleLantern()
    {
        if (arm == null || animator == null) return;

        isLanternActive = !isLanternActive;
        isAnimating = true;

        if (isLanternActive)
        {
            if (!arm.activeSelf)
                arm.SetActive(true);

            Debug.Log("Lantern On");
            animator.SetBool("isLanternActive", true);
            animator.Play("LanternUp");

            StartCoroutine(ResetAnimatingFlagAfterAnimation("LanternUp"));
        }
        else
        {
            Debug.Log("Lantern Off");
            animator.SetBool("isLanternActive", false);
            animator.Play("LanternDown");

            StartCoroutine(HideArmAfterAnimationCoroutine());
        }
    }

    IEnumerator HideArmAfterAnimationCoroutine()
    {
        while (!IsPlayingAnimation("LanternDown"))
        {
            yield return null;
        }

        while (IsPlayingAnimation("LanternDown"))
        {
            yield return null;
        }

        if (!isLanternActive && arm != null)
        {
            arm.SetActive(false);
        }

        isAnimating = false;
    }

    IEnumerator ResetAnimatingFlagAfterAnimation(string animationName)
    {
        while (!IsPlayingAnimation(animationName))
        {
            yield return null;
        }

        while (IsPlayingAnimation(animationName))
        {
            yield return null;
        }

        isAnimating = false;
    }

    bool IsPlayingAnimation(string animationName)
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f;
    }

    public void PickUpLantern()
    {
        hasLantern = true;
        isLanternActive = true;

        if (arm != null)
            arm.SetActive(true);

        if (animator != null)
            animator.SetBool("isLanternActive", true);
    }

    public bool IsLanternActive()
    {
        return isLanternActive;
    }

    public bool HasLantern()
{
    return hasLantern;
}

}
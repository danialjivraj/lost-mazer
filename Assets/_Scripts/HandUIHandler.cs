using UnityEngine;

public class HandUIHandler : MonoBehaviour
{
    public GameObject handUI;
    private PauseMenu pauseMenu;

    private void Start()
    {
        if (handUI != null)
        {
            handUI.SetActive(false);
        }

        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void ShowHandUI()
    {
        //Debug.Log($"ShowHandUI called by {name}. Stack trace:\n{StackTraceUtility.ExtractStackTrace()}");
        if (handUI != null && !IsGamePaused())
        {
            handUI.SetActive(true);
        }
    }

    public void HideHandUI()
    {
        if (handUI != null)
        {
            handUI.SetActive(false);
        }
    }

    public bool IsHandUIActive()
    {
        return handUI != null && handUI.activeInHierarchy;
    }

    public bool IsGamePaused()
    {
        return pauseMenu != null && pauseMenu.isPaused;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    public GameObject handUI;            
    public PickUpNotification doorNotification; 
    public GameObject invKey;            
    public GameObject fadeFX;            
    public string nextSceneName;         

    public int levelToUnlock = 2; 

    private bool inReach;
    private bool isTransitioning = false;

    void Start()
    {
        if (handUI != null) handUI.SetActive(false);
        if (doorNotification != null) doorNotification.ResetNotification();
        if (invKey != null) invKey.SetActive(false);
        if (fadeFX != null) fadeFX.SetActive(false);
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
            if (doorNotification != null)
                doorNotification.ResetNotification();
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact") && !isTransitioning)
        {
            if (!invKey.activeInHierarchy)
            {
                // if player does not have the key
                if (doorNotification != null)
                {
                    doorNotification.ShowNotification();
                }
            }
            else
            {
                isTransitioning = true;
                
                // save & log score
                ScoreManager.instance.SaveScore();
                ScoreManager.instance.LogScores();

                if (handUI != null)
                    handUI.SetActive(false);
                if (doorNotification != null)
                    doorNotification.ResetNotification();
                if (fadeFX != null)
                    fadeFX.SetActive(true);

                // unlocks next level directly within the Door script
                int currentLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
                if (levelToUnlock > currentLevel)
                {
                    Debug.Log("Unlocked level: " + levelToUnlock);
                    PlayerPrefs.SetInt("UnlockedLevel", levelToUnlock);
                    PlayerPrefs.Save();
                }

                StartCoroutine(LoadNextScene());
            }
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(nextSceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour
{
    public string chestId;

    private GameObject OB;
    public HandUIHandler handUIHandler;
    public GameObject[] objectsToActivate;

    public AudioSource chestOpenSound;
    public float soundVolume = 1.0f;

    private bool inReach;
    private bool isChestOpen = false;
    public PickUpNotification pickUpNotification;
    public List<SubtitleData> subtitles;

    void Awake()
    {
        if (string.IsNullOrEmpty(chestId))
        {
            chestId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;
        }
    }

    void Start()
    {
        OB = this.gameObject;

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }

        if (chestOpenSound != null)
        {
            chestOpenSound.volume = soundVolume;
        }
        else
        {
            Debug.LogWarning("Chest open sound AudioSource is not assigned.");
        }

        LoadChestState();
    }

    void LoadChestState()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            ChestState chestState = data.chestStates.Find(c => c.chestId == chestId);
            if (chestState != null && chestState.isOpen)
            {
                OpenChest(false, chestState.itemPickedUpStates);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !isChestOpen)
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

        if (inReach && Input.GetButtonDown("Interact") && !isChestOpen)
        {
            OpenChest();
        }
    }

    void OpenChest(bool playSound = true, List<bool> itemPickedUpStates = null)
    {
        if (playSound)
        {
            if (chestOpenSound != null && !chestOpenSound.isPlaying)
            {
                chestOpenSound.Play();
            }
            else
            {
                Debug.LogWarning("Chest open sound AudioSource is not assigned or is already playing.");
            }
        }
        
        if (handUIHandler != null)
            handUIHandler.HideHandUI();

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            bool pickedUp = itemPickedUpStates != null 
                            && itemPickedUpStates.Count > i 
                            && itemPickedUpStates[i];
            if (pickedUp)
            {
                if (objectsToActivate[i] != null)
                    Destroy(objectsToActivate[i]);
            }
            else
            {
                if (objectsToActivate[i] != null)
                    objectsToActivate[i].SetActive(true);
            }
        }

        if (OB.GetComponent<Animator>() != null)
            OB.GetComponent<Animator>().SetBool("open", true);

        if (OB.GetComponent<BoxCollider>() != null)
            OB.GetComponent<BoxCollider>().enabled = false;

        isChestOpen = true;

        if (pickUpNotification != null)
            pickUpNotification.ShowNotification();

        SubtitleManager.Instance.ResetSubtitles();
        foreach (SubtitleData subtitle in subtitles)
        {
            SubtitleManager.Instance.EnqueueSubtitle(subtitle);
        }
    }


    void SaveChestState()
    {
        GameStateData data = SaveLoadManager.LoadGame() ?? new GameStateData();
        ChestState chestState = data.chestStates.Find(c => c.chestId == chestId);
        if (chestState == null)
        {
            chestState = new ChestState { chestId = chestId };
            data.chestStates.Add(chestState);
        }
        chestState.isOpen = isChestOpen;
        SaveLoadManager.SaveGame(data);
    }

    public bool GetChestState()
    {
        return isChestOpen;
    }
}
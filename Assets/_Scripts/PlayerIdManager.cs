using UnityEngine;
using System;

public class PlayerIdManager : MonoBehaviour
{
    public static PlayerIdManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetPlayerId()
    {
        if (PlayerPrefs.HasKey("PlayerId"))
        {
            return PlayerPrefs.GetString("PlayerId");
        }
        else
        {
            string newPlayerId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerId", newPlayerId);
            PlayerPrefs.Save();
            return newPlayerId;
        }
    }
}

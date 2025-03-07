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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("PlayerIdManager");
            instance = go.AddComponent<PlayerIdManager>();
            DontDestroyOnLoad(go);
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

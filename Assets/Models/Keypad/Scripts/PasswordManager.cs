using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    public static int CurrentPassword { get; private set; }
    
    [SerializeField] private int minPassword = 1000;
    [SerializeField] private int maxPassword = 9999;

    void Awake()
    {
        if (SaveLoadManager.SaveExists())
        {
            GameStateData data = SaveLoadManager.LoadGame();
            if (data != null && data.doorPassword != 0)
            {
                CurrentPassword = data.doorPassword;
                Debug.Log("Loaded saved door password: " + CurrentPassword);
                return;
            }
        }
        
        CurrentPassword = Random.Range(minPassword, maxPassword + 1);
        Debug.Log("New Password: " + CurrentPassword);
    }
}

using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    public static int CurrentPassword { get; private set; }
    
    [SerializeField] private int minPassword = 1000;
    [SerializeField] private int maxPassword = 9999;

    void Awake()
    {
        CurrentPassword = Random.Range(minPassword, maxPassword + 1);
        Debug.Log("New Password: " + CurrentPassword);
    }
}

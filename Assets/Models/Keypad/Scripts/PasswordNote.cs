using UnityEngine;
using UnityEngine.UI;

public class PasswordNote : MonoBehaviour
{
    [SerializeField] private Text noteText;
    [SerializeField] private Text readableNoteText;

    void Start()
    {
        noteText.text = "The door code is: " + PasswordManager.CurrentPassword;
        readableNoteText.text = "The door code is: " + PasswordManager.CurrentPassword;
    }
}

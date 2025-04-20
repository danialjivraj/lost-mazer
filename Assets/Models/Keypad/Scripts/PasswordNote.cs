using UnityEngine;
using UnityEngine.UI;

public class PasswordNote : MonoBehaviour
{
    [SerializeField] private Text noteText;
    [SerializeField] private Text readableNoteText;

    [TextArea(8, 20)]
    [SerializeField]
    private string contentText = @"I saw him. The Red Man. Up close, closer than I ever wanted. There was a keypad on the wall. I tried a number and the screen blinked red. Then I heard him, heavy steps, that awful dragging sound. I managed to duck behind a gravestone in the corner before he could see me. I saw him walking up to the keypad. He pressed four numbers:

{password}

The wall slid open, and he stepped inside. I must’ve gasped or moved as he turned around and saw me. I ran through halls I didn’t know… downstairs that shouldn’t exist. I think I found his lair… I see knives, bones, trophies. I’ll try the passcode when I have the chance. Maybe that number is the key to ending all of these nightmares.";

    [TextArea(12, 30)]
    [SerializeField]
    private string readeableText = @"30th of October, 1981

I saw him. The Red Man. Up close, closer than I ever wanted. There was a keypad on the wall. I tried a number and the screen blinked red. Then I heard him, heavy steps, that awful dragging sound. I managed to duck behind a gravestone in the corner before he could see me. I saw him walking up to the keypad. He pressed four numbers:

{password}

The wall slid open, and he stepped inside. I must’ve gasped or moved as he turned around and saw me. I ran through halls I didn’t know… downstairs that shouldn’t exist. I think I found his lair… I see knives, bones, trophies. I’ll try the passcode when I have the chance. Maybe that number is the key to ending all of these nightmares.

Eleanor Blackwood
—7 of 7";

    void Start()
    {
        int pw = PasswordManager.CurrentPassword;
        string formatted = string.Join(" - ", pw.ToString().ToCharArray());

        noteText.text = contentText.Replace("{password}", formatted);

        readableNoteText.text = readeableText.Replace("{password}", formatted);
    }
}

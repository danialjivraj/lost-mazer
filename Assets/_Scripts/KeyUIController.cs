using UnityEngine;

public class KeyUIController : MonoBehaviour
{
    public GameObject noKeyImage;
    public GameObject keyCollectedImage;

    void Update()
    {
        if (KeyController.instance != null)
        {
            if (KeyController.instance.hasKey)
            {
                noKeyImage.SetActive(false);
                keyCollectedImage.SetActive(true);
            }
            else
            {
                noKeyImage.SetActive(true);
                keyCollectedImage.SetActive(false);
            }
        }
    }
}

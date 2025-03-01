using UnityEngine;

public class KeyController : MonoBehaviour
{
    public static KeyController instance;

    public bool hasKey = false;
    public GameObject keyObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null && data.playerHasKey)
        {
            PickUpKey();
        }
    }

    public void PickUpKey()
    {
        hasKey = true;
        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
    }
}

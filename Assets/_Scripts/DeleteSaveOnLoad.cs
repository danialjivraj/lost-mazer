using System.Collections;
using UnityEngine;

public class DeleteSaveOnLoad : MonoBehaviour
{
    private IEnumerator Start()
    {
        // basically everything else loads first then this runs, hopefully, lmao!
        yield return new WaitForEndOfFrame();
        
        SaveLoadManager.DeleteSave();
        PickupItemManager.pickedUpItemIds.Clear();
        Debug.Log("Save file deleted after level loaded.");
    }
}

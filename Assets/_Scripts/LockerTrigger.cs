using UnityEngine;

public class LockerTrigger : MonoBehaviour
{
    public bool isPlayerInside { get; private set; } = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Inside locker");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log("Outside locker");
        }
    }
}

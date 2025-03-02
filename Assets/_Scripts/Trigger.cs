using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool destroyOnTriggerEnter;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    public string triggerId;
    private bool hasTriggered = false;

    void Awake()
    {
        if (string.IsNullOrEmpty(triggerId))
        {
            Vector3 pos = transform.position;
            triggerId = $"{gameObject.name}_{pos.x:F2}_{pos.y:F2}_{pos.z:F2}";
        }
    }

    void Start()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null)
        {
            TriggerState savedState = data.triggerStates.Find(s => s.triggerId == triggerId);
            if (savedState != null)
            {
                SetTriggeredState(savedState.hasTriggered);

                if (savedState.hasTriggered)
                {
                    foreach (GameObject obj in targetObjects)
                    {
                        if (obj != null)
                            obj.SetActive(true);
                    }
                }

                if (savedState.hasTriggered && destroyOnTriggerEnter)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            onTriggerEnter.Invoke();

            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            if (destroyOnTriggerEnter)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerExit.Invoke();
        }
    }

    public bool GetTriggeredState() => hasTriggered;

    public void SetTriggeredState(bool state)
    {
        hasTriggered = state;
    }
}

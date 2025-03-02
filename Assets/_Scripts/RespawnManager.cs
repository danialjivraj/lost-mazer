using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private Dictionary<string, float> respawnTimers = new Dictionary<string, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator RespawnItem(GameObject item, float delay, string itemId)
    {
        respawnTimers[itemId] = Time.time + delay;
        float remaining = delay;
        while (remaining > 0)
        {
            yield return null;
            remaining = respawnTimers[itemId] - Time.time;
        }
        respawnTimers.Remove(itemId);
        item.SetActive(true);
    }

    public float GetRemainingRespawnTime(string itemId)
    {
        if (respawnTimers.ContainsKey(itemId))
        {
            return Mathf.Max(0, respawnTimers[itemId] - Time.time);
        }
        return 0;
    }
}

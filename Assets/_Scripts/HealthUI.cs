using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RawImage[] hearts;

    void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.CurrentHealth)
                hearts[i].enabled = true; 
            else
                hearts[i].enabled = false;
        }
    }
}

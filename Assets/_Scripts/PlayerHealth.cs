using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }
    public float deathDelay = 0.5f;
    public GameObject fadeoutEffect;

    private void Start()
    {
        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null && data.playerHealth > 0)
        {
            currentHealth = data.playerHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // stores current level of the game so player can retry the specific level
        LevelManager.SetLastLevel(SceneManager.GetActiveScene().name);

        if (fadeoutEffect != null)
        {
            fadeoutEffect.SetActive(true);
        }

        Invoke("LoadDeathScreen", deathDelay);
    }

    private void LoadDeathScreen()
    {
        SceneManager.LoadScene("DeathScreen");
    }
}
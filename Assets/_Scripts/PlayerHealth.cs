using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }
    public float deathDelay = 0.5f;
    public GameObject fadeoutEffect;
    
    public AudioClip[] gruntClips;
    private AudioSource audioSource;

    public AudioMixerGroup playerSFXGroup;

    public GameObject bloodOverlayCanvas;
    public float bloodFadeInTime = 0.5f;
    public float bloodDisplayDuration = 2f;
    public float bloodFadeOutTime = 0.5f;

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
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (playerSFXGroup != null)
        {
            audioSource.outputAudioMixerGroup = playerSFXGroup;
        }
        
        if(bloodOverlayCanvas != null)
        {
            bloodOverlayCanvas.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (TriggerCutscene.isCutsceneActive)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("health: " + currentHealth);
        
        PlayGruntSound();
        
        if(bloodOverlayCanvas != null)
        {
            StartCoroutine(ShowBloodOverlay());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void PlayGruntSound()
    {
        if (gruntClips != null && gruntClips.Length > 0)
        {
            int randomIndex = Random.Range(0, gruntClips.Length);
            AudioClip clipToPlay = gruntClips[randomIndex];
            audioSource.PlayOneShot(clipToPlay);
        }
    }
    
    private IEnumerator ShowBloodOverlay()
    {
        bloodOverlayCanvas.SetActive(true);

        CanvasGroup canvasGroup = bloodOverlayCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = bloodOverlayCanvas.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;

        // fades in
        float timer = 0f;
        while(timer < bloodFadeInTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / bloodFadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // remains visible for the duration
        yield return new WaitForSeconds(bloodDisplayDuration);

        // fades out
        timer = 0f;
        while(timer < bloodFadeOutTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / bloodFadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        bloodOverlayCanvas.SetActive(false);
    }

    private void Die()
    {
        // stores the current level so the player can retry the specific level
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
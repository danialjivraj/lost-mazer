using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MultiPlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 3;
    public RawImage[] hearts;
    int currentHealth;

    public AudioClip[] gruntClips;
    private AudioSource audioSource;
    public AudioMixerGroup playerSFXGroup;

    void Awake()
    {
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (playerSFXGroup != null)
            audioSource.outputAudioMixerGroup = playerSFXGroup;

        if (photonView.IsMine)
            UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(int amount, PhotonMessageInfo info)
    {
        if (MultiplayerScoreManager.Instance != null &&
            MultiplayerScoreManager.Instance.IsRoundOver)
            return;

        if (!photonView.IsMine) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);

        PlayGruntSound();

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die(info.Sender);
    }

    void UpdateHealthUI()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = (i < currentHealth);
    }

    void Die(Player winner)
    {
        if (!photonView.IsMine) return;

        object raw;
        winner.CustomProperties.TryGetValue("Team", out raw);
        int winnerTeam = raw != null ? (int)raw : 0;
        bool blueWon = (winnerTeam == 0);

        MultiplayerScoreManager.Instance.EndRound(blueWon);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (photonView.IsMine) UpdateHealthUI();
    }

    private void PlayGruntSound()
    {
        if (!photonView.IsMine) 
            return;

        if (gruntClips != null && gruntClips.Length > 0 && audioSource != null)
        {
            int idx = Random.Range(0, gruntClips.Length);
            audioSource.PlayOneShot(gruntClips[idx]);
        }
    }
}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 3;
    public RawImage[] hearts;
    int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        if (photonView.IsMine) UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(int amount, PhotonMessageInfo info)
    {
        if (MultiplayerScoreManager.Instance != null &&
            MultiplayerScoreManager.Instance.IsRoundOver)
            return;

        if (!photonView.IsMine) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
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
}

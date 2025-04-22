using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 3;
    public RawImage[] hearts;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;

        if (photonView.IsMine)
            UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(int amount, PhotonMessageInfo info)
    {
        if (!photonView.IsMine) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die(info.Sender);
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = (i < currentHealth);
        }
    }

    private void Die(Player winner)
    {
    }
}

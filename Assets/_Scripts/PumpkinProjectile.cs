using Photon.Pun;
using UnityEngine;

public class PumpkinProjectile : MonoBehaviourPun
{
    public int damage = 1;
    public float lifeTime = 5f;

    bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!photonView.IsMine || hasHit) return;
        if (MultiplayerScoreManager.Instance != null &&
            MultiplayerScoreManager.Instance.IsRoundOver)
            return;

        hasHit = true;
        var targetPv = col.collider.GetComponent<PhotonView>();
        if (targetPv != null &&
            targetPv.OwnerActorNr != photonView.OwnerActorNr)
        {
            targetPv.RPC("TakeDamage", targetPv.Owner, damage);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}

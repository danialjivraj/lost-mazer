using Photon.Pun;
using UnityEngine;

public class PumpkinProjectile : MonoBehaviourPun
{
    public int damage = 1;
    public float lifeTime = 5f;

    private bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!photonView.IsMine || hasHit) return;
        hasHit = true;

        var targetPv = col.collider.GetComponent<PhotonView>();
        if (targetPv != null)
        {
            // does not damage own player
            if (targetPv.OwnerActorNr != photonView.OwnerActorNr)
            {
                targetPv.RPC(
                    "TakeDamage",
                    targetPv.Owner, 
                    damage
                );
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}

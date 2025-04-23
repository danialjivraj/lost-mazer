using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PumpkinProjectile : MonoBehaviourPun
{
    public int damage = 1;
    public float lifeTime = 5f;

    public AudioSource throwSource;
    public AudioSource hitSource;

    bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        if (throwSource != null)
            throwSource.Play();
    }

    void OnCollisionEnter(Collision col)
    {
        if (!photonView.IsMine || hasHit) return;
        if (MultiplayerScoreManager.Instance?.IsRoundOver ?? false) return;

        hasHit = true;

        // send damage RPC
        var targetPv = col.collider.GetComponent<PhotonView>()
                     ?? col.collider.GetComponentInParent<PhotonView>();
        if (targetPv != null && targetPv.OwnerActorNr != photonView.OwnerActorNr)
            targetPv.RPC("TakeDamage", targetPv.Owner, damage);

        // hide visuals & stop physics
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>())  c.enabled = false;
        if (TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;

        // hit‐sound RPC with impact point
        Vector3 hitPoint = col.GetContact(0).point;
        photonView.RPC(nameof(RPC_PlayHitSound), RpcTarget.All, hitPoint);

        // destroy after the hit clip finishes
        float delay = (hitSource != null && hitSource.clip != null)
                    ? hitSource.clip.length
                    : 0f;
        StartCoroutine(DestroyAfterDelay(delay));
    }

    [PunRPC]
    void RPC_PlayHitSound(Vector3 pos)
    {
        if (hitSource == null) return;

        hitSource.transform.position = pos;
        hitSource.Play();
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }
}

using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Rigidbody[] ragdollRigidbodies;
    public Collider[] ragdollColliders;

    public Collider mainCollider;

    void Start()
    {
        if (ragdollRigidbodies != null)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = true;
            }
        }
        
        if (ragdollColliders != null)
        {
            foreach (Collider col in ragdollColliders)
            {
                col.enabled = false;
            }
        }

        if (mainCollider != null)
        {
            mainCollider.enabled = true;
        }
    }

    public void EnableRagdoll()
    {
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }

        if (ragdollRigidbodies != null)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }
        }
        if (ragdollColliders != null)
        {
            foreach (Collider col in ragdollColliders)
            {
                col.enabled = true;
            }
        }
    }
}

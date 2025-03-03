using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    public string ragdollId;

    public Rigidbody[] ragdollRigidbodies;
    public Collider[] ragdollColliders;
    public Collider mainCollider;
    public bool physicsActivated = false;

    void Awake()
    {
        if (string.IsNullOrEmpty(ragdollId))
        {
            Vector3 pos = transform.position;
            ragdollId = gameObject.name + "_" + pos.x.ToString("F2") + "_" + pos.y.ToString("F2") + "_" + pos.z.ToString("F2");
        }
    }

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

        GameStateData data = SaveLoadManager.LoadGame();
        if (data != null && data.ragdollStates != null && data.ragdollStates.Count > 0)
        {
            RagdollState foundState = data.ragdollStates.Find(r => r.ragdollId == ragdollId);
            if (foundState != null)
            {
                StartCoroutine(ApplyRagdollStateCoroutine(foundState));
            }
        }
    }

    // this is used in combination with the trigger script, as it goes from "static" to ragdoll when the trigger activates 
    public void EnableRagdoll()
    {
        physicsActivated = true;

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

    public RagdollState SaveRagdollState()
    {
        RagdollState ragdollState = new RagdollState
        {
            ragdollId = ragdollId,
            physicsActivated = physicsActivated,
            partStates = new List<RagdollPartState>()
        };

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            RagdollPartState partState = new RagdollPartState
            {
                partName = rb.gameObject.name,
                position = rb.position,
                rotation = rb.rotation
            };
            ragdollState.partStates.Add(partState);
        }
        return ragdollState;
    }

    private IEnumerator ApplyRagdollStateCoroutine(RagdollState savedRagdollState)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (RagdollPartState part in savedRagdollState.partStates)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb.gameObject.name == part.partName)
                {
                    rb.position = part.position;
                    rb.rotation = part.rotation;
                    break;
                }
            }
        }

        yield return null;

        physicsActivated = savedRagdollState.physicsActivated;

        if (physicsActivated)
        {
            EnableRagdoll();
        }
    }
}

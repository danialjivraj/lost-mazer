using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float sightDistance = 10f;
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;

    private enum EnemyState { Idle, Walk, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

    private bool isChasingAnimation = false;

    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 20;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        agent.angularSpeed = 360f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 1.5f;

        SetDestinationToWaypoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                idleTimer += Time.deltaTime;
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                PlaySound(idleSound);

                if (idleTimer >= idleTime)
                {
                    NextWaypoint();
                }
                CheckForPlayerDetection();
                break;

            case EnemyState.Walk:
                idleTimer = 0f;
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                PlaySound(walkingSound);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Idle;
                }
                CheckForPlayerDetection();
                break;

            case EnemyState.Chase:
                idleTimer = 0f;
                agent.speed = chaseSpeed;

                agent.SetDestination(player.position);

                animator.SetBool("IsWalking", false);
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsAttacking", false);
                PlaySound(chasingSound);

                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer > 10f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
                }

                if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time >= nextAttackTime)
                {
                    currentState = EnemyState.Attack;
                    StartCoroutine(AttackPlayer());
                }

                // If player is too far, return to patrol
                if (Vector3.Distance(transform.position, player.position) > sightDistance)
                {
                    currentState = EnemyState.Walk;
                    agent.speed = walkSpeed;
                }
                break;

            case EnemyState.Attack:
                agent.isStopped = true;
                break;
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        currentState = EnemyState.Attack;
        transform.LookAt(player.position);
        animator.SetBool("IsAttacking", true);
        nextAttackTime = Time.time + attackCooldown;

        yield return new WaitForSeconds(0.3f);

        Vector3 attackDirection = (player.position - transform.position).normalized;
        agent.SetDestination(transform.position + attackDirection * 0.5f);

        if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(0.4f);

        agent.isStopped = true;
        yield return new WaitForSeconds(1f); // Stop time

        // Resume Chasing
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        currentState = EnemyState.Chase;

        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(player.position);
    }

    private void CheckForPlayerDetection()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;

        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chase;
                Debug.Log("Player detected!");
            }
        }
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (!audioSource.isPlaying || audioSource.clip != soundClip)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }

    private void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
        agent.speed = walkSpeed;
        animator.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, player.position);
    }
}
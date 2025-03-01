using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints;
    public float idleTime = 4f;
    public float walkSpeed = 3.5f;
    public float chaseSpeed = 6.5f;
    public float chaseDelay = 2f;
    private float chaseEndTime = 0f;
    public float sightDistance = 30f;
    public float lanternSightMultiplier = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 1;
    private bool hasDealtDamage = false;
    private float nextAttackTime = 0f;
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;
    private LanternController playerLantern;

    public float hearingRange = 20f;
    public float runningSoundMultiplier = 2f;
    private Vector3 lastHeardPosition;
    private bool isInvestigating = false;

    private LockerInteraction[] lockerInteractions;

    private enum EnemyState { Idle, Walk, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        playerLantern = player.GetComponentInChildren<LanternController>();

        agent.stoppingDistance = 1.5f;

        lockerInteractions = FindObjectsOfType<LockerInteraction>();

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.OnFootstep.AddListener(OnPlayerFootstep);
        }
        
        GameStateData gameState = SaveLoadManager.LoadGame();
        if (gameState != null && gameState.enemyStates.Count > 0)
        {
            LoadEnemyState(gameState.enemyStates[0]);
        }
        else
        {
            SetDestinationToWaypoint();
        }
    }

    private void OnPlayerFootstep(Vector3 position, float volume)
    {
        if (LockerInteraction.IsAnyLockerHiding())
            return;

        if (currentState == EnemyState.Idle || currentState == EnemyState.Walk)
        {
            float distanceToSound = Vector3.Distance(transform.position, position);
            float effectiveHearingRange = hearingRange * (volume * runningSoundMultiplier);
            if (distanceToSound <= effectiveHearingRange)
            {
                lastHeardPosition = position;
                isInvestigating = true;
                currentState = EnemyState.Walk;
                agent.SetDestination(lastHeardPosition);
            }
        }
    }

    private void Update()
    {
        float currentSightDistance = sightDistance;
        if (playerLantern != null && playerLantern.IsLanternActive())
        {
            currentSightDistance *= lanternSightMultiplier;
        }
        
        // if the player is now hiding, cancel investigation/chase.
        if (LockerInteraction.IsAnyLockerHiding())
        {
            // Cancel chase/investigation
            if (currentState == EnemyState.Chase || currentState == EnemyState.Attack || isInvestigating)
            {
                isInvestigating = false;
                lastHeardPosition = Vector3.zero;
                currentState = EnemyState.Idle;
                agent.speed = walkSpeed;
                SetDestinationToWaypoint();
                return;
            }
        }
        
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
                CheckForPlayerDetection(currentSightDistance);
                break;
            case EnemyState.Walk:
                idleTimer = 0f;
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                PlaySound(walkingSound);

                // moves toward the last heard position if investigating
                if (isInvestigating)
                {
                    agent.SetDestination(lastHeardPosition);

                    // stops investigating when reaching the destination.
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        isInvestigating = false;
                        currentState = EnemyState.Idle;
                    }
                }
                else
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        currentState = EnemyState.Idle;
                    }
                }
                CheckForPlayerDetection(currentSightDistance);
                break;
            case EnemyState.Chase:
                idleTimer = 0f;
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsAttacking", false);
                PlaySound(chasingSound);

                if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time >= nextAttackTime)
                {
                    currentState = EnemyState.Attack;
                    StartCoroutine(AttackPlayer());
                }

                if (Vector3.Distance(transform.position, player.position) > currentSightDistance)
                {
                    if (chaseEndTime == 0f)
                    {
                        chaseEndTime = Time.time + chaseDelay;
                    }
                    else if (Time.time >= chaseEndTime)
                    {
                        currentState = EnemyState.Walk;
                        agent.speed = walkSpeed;
                        chaseEndTime = 0f;
                    }
                }
                else
                {
                    chaseEndTime = 0f;
                }
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                break;
        }
    }

    private IEnumerator AttackPlayer()
    {
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
        yield return new WaitForSeconds(1f);

        // resume chasing
        animator.SetBool("IsAttacking", false);
        currentState = EnemyState.Chase;
        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(player.position);
    }

    private void CheckForPlayerDetection(float currentSightDistance)
    {
        if (LockerInteraction.IsAnyLockerHiding())
            return;

        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;
        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, currentSightDistance))
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

    public EnemyStateData GetEnemyState() {
        EnemyStateData state = new EnemyStateData();
        state.position = transform.position;
        state.rotation = transform.rotation;

        state.destination = agent.destination;
        state.agentSpeed = agent.speed;

        state.currentWaypointIndex = currentWaypointIndex;
        state.currentState = (int) currentState;

        state.idleTimer = idleTimer;
        state.lastHeardPosition = lastHeardPosition;
        state.isInvestigating = isInvestigating;
        state.chaseEndTime = chaseEndTime;
        state.nextAttackTime = nextAttackTime;

        state.animIsWalking = animator.GetBool("IsWalking");
        state.animIsChasing = animator.GetBool("IsChasing");
        state.animIsAttacking = animator.GetBool("IsAttacking");

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        state.currentAnimNormalizedTime = animInfo.normalizedTime;
        state.currentAnimStateName = animInfo.IsName("Enemy_Idle") ? "Enemy_Idle" :
                                    animInfo.IsName("Enemy_Walk") ? "Enemy_Walk" :
                                    animInfo.IsName("Enemy_Run") ? "Enemy_Run" :
                                    animInfo.IsName("Enemy_Attack") ? "Enemy_Attack" : "Unknown";

        return state;
    }

    public void LoadEnemyState(EnemyStateData state) {
        if(agent != null) {
            agent.Warp(state.position);
        } else {
            transform.position = state.position;
        }
        
        transform.rotation = state.rotation;

        agent.speed = state.agentSpeed;
        agent.SetDestination(state.destination);

        currentWaypointIndex = state.currentWaypointIndex;
        currentState = (EnemyState) state.currentState;

        idleTimer = state.idleTimer;
        lastHeardPosition = state.lastHeardPosition;
        isInvestigating = state.isInvestigating;
        chaseEndTime = state.chaseEndTime;
        nextAttackTime = state.nextAttackTime;

        animator.SetBool("IsWalking", state.animIsWalking);
        animator.SetBool("IsChasing", state.animIsChasing);
        animator.SetBool("IsAttacking", state.animIsAttacking);

        animator.Play(state.currentAnimStateName, 0, state.currentAnimNormalizedTime);

        if (state.currentAnimStateName == "Enemy_Attack")
        {
            currentState = EnemyState.Chase;
            animator.SetBool("IsAttacking", false);
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(player.position);
            nextAttackTime = Time.time;
        }
    }

    private void OnDrawGizmos()
    {
        float currentSightDistance = sightDistance;
        if (playerLantern != null && playerLantern.IsLanternActive())
        {
            currentSightDistance *= lanternSightMultiplier;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentSightDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        if (isInvestigating)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lastHeardPosition);
        }
    }
}
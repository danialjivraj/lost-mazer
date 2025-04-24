using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // movement and navigation
    public Transform[] waypoints;
    public float walkSpeed = 3.5f;
    public float chaseSpeed = 6.5f;
    public float chaseDelay = 2f;
    public float idleTime = 4f;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private float idleTimer = 0f;

    // player detection and interaction
    public float sightDistance = 30f;
    public float lanternSightMultiplier = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 1;
    private float nextAttackTime = 0f;
    private int detectionLayerMask;
    private Transform player;
    private LanternController playerLantern;

    public float elevationThreshold = 0.5f;

    // hearing and investigation
    public float hearingRange = 20f;
    public float runningSoundMultiplier = 2f;
    private Vector3 lastHeardPosition;
    private bool isInvestigating = false;

    // audio
    public AudioSource idleSound;
    public AudioSource walkingSound;
    public AudioSource chasingSound;
    public AudioSource attackSound;
    public AudioSource chaseTrack;
    public float chaseTrackFadeSpeed = 1f;
    private float chaseTrackTargetVolume;

    // animation
    private Animator animator;

    // state management
    private enum EnemyState { Idle, Walk, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;
    private float chaseEndTime = 0f;
    // for logging state changes
    private EnemyState previousState = EnemyState.Idle;

    // locker interaction
    private LockerInteraction[] lockerInteractions;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerLantern = player.GetComponentInChildren<LanternController>();
        lockerInteractions = FindObjectsOfType<LockerInteraction>();

        chaseTrackTargetVolume = chaseTrack.volume;
        agent.stoppingDistance = 1.5f;

        detectionLayerMask = Physics.DefaultRaycastLayers;

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

    void Update()
    {
        if (TriggerCutscene.freezeEnemiesActive)
        {
            agent.isStopped = true;
            animator.enabled = false;
            return;
        }
        else
        {
            if (agent.isStopped) agent.isStopped = false;
            if (!animator.enabled) animator.enabled = true;
        }

        float currentSightDistance = sightDistance;
        if (playerLantern != null && playerLantern.IsLanternActive())
            currentSightDistance *= lanternSightMultiplier;

        if (LockerInteraction.IsPlayerHidingInAnyLocker())
        {
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

        ManageChaseTrack();

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState(currentSightDistance);
                break;
            case EnemyState.Walk:
                HandleWalkState(currentSightDistance);
                break;
            case EnemyState.Chase:
                HandleChaseState(currentSightDistance);
                break;
            case EnemyState.Attack:
                break;
        }

        if (currentState != previousState)
        {
            Debug.Log("Enemy State: " + currentState);
            previousState = currentState;
        }
    }

    private void OnPlayerFootstep(Vector3 position, float volume)
    {
        if (LockerInteraction.IsPlayerHidingInAnyLocker())
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
                agent.SetDestination(position);
            }
        }
    }

    private void HandleIdleState(float currentSightDistance)
    {
        idleTimer += Time.deltaTime;
        SetAnimatorFlags(idle: true);
        PlaySound(idleSound);

        if (idleTimer >= idleTime) NextWaypoint();
        CheckForPlayerDetection(currentSightDistance);
    }

    private void HandleWalkState(float currentSightDistance)
    {
        idleTimer = 0f;
        SetAnimatorFlags(walking: true);
        PlaySound(walkingSound);

        if (isInvestigating)
        {
            agent.SetDestination(lastHeardPosition);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isInvestigating = false;
                currentState = EnemyState.Idle;
            }
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = EnemyState.Idle;
        }

        CheckForPlayerDetection(currentSightDistance);
    }

    private void HandleChaseState(float currentSightDistance)
    {
        idleTimer = 0f;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        SetAnimatorFlags(chasing: true);
        PlaySound(chasingSound);

        int chaseLayer = animator.GetLayerIndex("chasing with arm up");
        if (chaseLayer == -1)
        {
            Debug.LogError("Missing animator layer");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // flat XZ distance
        Vector3 enemyFlat = new Vector3(transform.position.x, player.position.y, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x, player.position.y, player.position.z);
        float horizontalDistance = Vector3.Distance(enemyFlat, playerFlat);

        if (distanceToPlayer <= 10f)
        {
            float targetWeight = 0.6f + Mathf.PingPong(Time.time, 0.4f);
            float currentWeight = animator.GetLayerWeight(chaseLayer);
            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime);
            animator.SetLayerWeight(chaseLayer, newWeight);
        }
        else
        {
            float currentWeight = animator.GetLayerWeight(chaseLayer);
            float newWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * 5f);
            animator.SetLayerWeight(chaseLayer, newWeight);
        }

        float verticalDifference = Mathf.Abs(player.position.y - transform.position.y);
        float distanceToUse = (verticalDifference > elevationThreshold) ? horizontalDistance : distanceToPlayer;

        if (distanceToUse <= attackRange && Time.time >= nextAttackTime)
        {
            currentState = EnemyState.Attack;
            StartCoroutine(AttackPlayer());
        }

        if (distanceToPlayer > currentSightDistance)
        {
            if (chaseEndTime == 0f)
                chaseEndTime = Time.time + chaseDelay;
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
    }

    private IEnumerator AttackPlayer()
    {
        currentState = EnemyState.Attack;
        transform.LookAt(player.position);
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsChasing", false);

        int runArmUpLayer = animator.GetLayerIndex("Enemy_ChaseArmUp");
        float originalRunArmUpWeight = animator.GetLayerWeight(runArmUpLayer);
        animator.SetLayerWeight(runArmUpLayer, 0f);

        nextAttackTime = Time.time + attackCooldown;
        PlaySound(attackSound);

        yield return new WaitForSeconds(0.3f);

        Vector3 attackDirection = (player.position - transform.position).normalized;
        agent.SetDestination(transform.position + attackDirection * 0.5f);

        float fullDistance = Vector3.Distance(transform.position, player.position);
        Vector3 diff = player.position - transform.position;
        diff.y = 0f;
        float flatDistance = diff.magnitude;
        float verticalDifference = Mathf.Abs(player.position.y - transform.position.y);
        float distanceToUse = (verticalDifference > elevationThreshold) ? flatDistance : fullDistance;

        if (distanceToUse <= attackRange + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.4f);

        agent.isStopped = true;
        yield return new WaitForSeconds(1f);

        animator.SetBool("IsAttacking", false);
        animator.SetLayerWeight(runArmUpLayer, originalRunArmUpWeight);

        currentState = EnemyState.Chase;
        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(player.position);
    }

    private void CheckForPlayerDetection(float currentSightDistance)
    {
        if (LockerInteraction.IsPlayerHidingInAnyLocker())
            return;

        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;
        if (Physics.Raycast(transform.position, playerDirection.normalized,
                            out hit, currentSightDistance, detectionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chase;
                Debug.Log("Player detected");
            }
        }
    }

    private void PlaySound(AudioSource soundSource)
    {
        if (soundSource != idleSound) idleSound.Stop();
        if (soundSource != walkingSound) walkingSound.Stop();
        if (soundSource != chasingSound) chasingSound.Stop();
        if (soundSource != attackSound) attackSound.Stop();

        if (Time.timeScale == 0f)
            return;

        if (!soundSource.isPlaying)
            soundSource.Play();
    }

    private void ManageChaseTrack()
    {
        if (Time.timeScale == 0f)
            return;

        if (currentState == EnemyState.Chase || currentState == EnemyState.Attack)
        {
            if (!chaseTrack.isPlaying)
            {
                chaseTrack.volume = 0f;
                chaseTrack.Play();
            }
            if (chaseTrack.volume < chaseTrackTargetVolume)
                chaseTrack.volume = Mathf.Min(chaseTrackTargetVolume,
                                              chaseTrack.volume + chaseTrackFadeSpeed * Time.deltaTime);
        }
        else if (chaseTrack.isPlaying)
        {
            chaseTrack.volume = Mathf.Max(0f,
                                          chaseTrack.volume - chaseTrackFadeSpeed * Time.deltaTime);
            if (chaseTrack.volume <= 0f)
            {
                chaseTrack.Stop();
                chaseTrack.volume = chaseTrackTargetVolume;
            }
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

    private void SetAnimatorFlags(bool idle = false, bool walking = false, bool chasing = false)
    {
        animator.SetBool("IsIdle", idle);
        animator.SetBool("IsWalking", walking);
        animator.SetBool("IsChasing", chasing);
        animator.SetBool("IsAttacking", false);
    }

    public EnemyStateData GetEnemyState()
    {
        EnemyStateData state = new EnemyStateData();
        state.position = transform.position;
        state.rotation = transform.rotation;
        state.destination = agent.destination;
        state.agentSpeed = agent.speed;
        state.currentWaypointIndex = currentWaypointIndex;
        state.currentState = (int)currentState;
        state.idleTimer = idleTimer;
        state.lastHeardPosition = lastHeardPosition;
        state.isInvestigating = isInvestigating;
        state.chaseEndTime = chaseEndTime;
        state.nextAttackTime = nextAttackTime;
        state.animIsIdle = animator.GetBool("IsIdle");
        state.animIsWalking = animator.GetBool("IsWalking");
        state.animIsChasing = animator.GetBool("IsChasing");
        state.animIsAttacking = animator.GetBool("IsAttacking");

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        state.currentAnimNormalizedTime = animInfo.normalizedTime;
        state.currentAnimStateName = animInfo.IsName("Enemy_Idle")   ? "Enemy_Idle"
                                  : animInfo.IsName("Enemy_Walk")   ? "Enemy_Walk"
                                  : animInfo.IsName("Enemy_Chase")  ? "Enemy_Chase"
                                  : animInfo.IsName("Enemy_Attack") ? "Enemy_Attack"
                                  : "Unknown";

        return state;
    }

    public void LoadEnemyState(EnemyStateData state)
    {
        if (agent != null)
            agent.Warp(state.position);
        else
            transform.position = state.position;

        transform.rotation = state.rotation;
        agent.speed = state.agentSpeed;
        agent.SetDestination(state.destination);
        currentWaypointIndex = state.currentWaypointIndex;
        currentState = (EnemyState)state.currentState;
        idleTimer = state.idleTimer;
        lastHeardPosition = state.lastHeardPosition;
        isInvestigating = state.isInvestigating;
        chaseEndTime = state.chaseEndTime;
        nextAttackTime = state.nextAttackTime;

        animator.SetBool("IsIdle", state.animIsIdle);
        animator.SetBool("IsWalking", state.animIsWalking);
        animator.SetBool("IsChasing", state.animIsChasing);
        animator.SetBool("IsAttacking", state.animIsAttacking);
        animator.Play(state.currentAnimStateName, 0, state.currentAnimNormalizedTime);

        if (currentState == EnemyState.Attack)
        {
            animator.SetBool("IsAttacking", false);
            currentState = EnemyState.Chase;
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(player.position);
        }
    }

    private void OnDrawGizmos()
    {
        float currentSightDistance = sightDistance;
        if (playerLantern != null && playerLantern.IsLanternActive())
            currentSightDistance *= lanternSightMultiplier;

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

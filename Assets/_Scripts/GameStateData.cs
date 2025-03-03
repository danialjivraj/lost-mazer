using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestState
{
    public string chestId;
    public bool isOpen;
    public List<bool> itemPickedUpStates = new List<bool>();
}

[System.Serializable]
public class PickupItemState
{
    public string itemId;
    public bool isPickedUp;
    public float remainingRespawnTime;
}

[System.Serializable]
public class LanternState
{
    public bool hasLantern;
    public bool isLanternActive;
}

[System.Serializable]
public class LockerState
{
    public string lockerId;
    public bool isOpen;
    public bool isPlayerInside;
    
    public float animNormalizedTime;
}

[System.Serializable]
public class EnemyStateData {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 destination;
    public float agentSpeed;

    public int currentWaypointIndex;
    public int currentState;

    public float idleTimer;
    public Vector3 lastHeardPosition;
    public bool isInvestigating;
    public float chaseEndTime;
    public float nextAttackTime;

    public bool animIsWalking;
    public bool animIsChasing;
    public bool animIsAttacking;
    public float currentAnimNormalizedTime;
    public string currentAnimStateName;
}

[System.Serializable]
public class TriggerState
{
    public string triggerId;
    public bool hasTriggered;
}

[System.Serializable]
public class RagdollPartState
{
    public string partName;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class RagdollState
{
    public string ragdollId;
    public bool physicsActivated;
    public List<RagdollPartState> partStates = new List<RagdollPartState>();
}

[System.Serializable]
public class GameStateData
{

    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Quaternion cameraRotation;
    
    public float rotationX;
    public float rotationY;
    public bool isCrouching;
    public bool isZoomed;
    public float currentHeight;
    public float cameraFOV;

    public int playerHealth;

    public int score;
    public bool playerHasKey;

    public bool isReadingNote;
    public bool isReadableViewActive;

    public List<ChestState> chestStates = new List<ChestState>();
    public List<PickupItemState> pickupItemStates = new List<PickupItemState>();

    public LanternState lanternState;

    public List<LockerState> lockerStates = new List<LockerState>();

    public List<EnemyStateData> enemyStates = new List<EnemyStateData>();

    public List<TriggerState> triggerStates = new List<TriggerState>();

    public List<RagdollState> ragdollStates = new List<RagdollState>();
}
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

    public List<ChestState> chestStates = new List<ChestState>();
    public List<PickupItemState> pickupItemStates = new List<PickupItemState>();

    public LanternState lanternState;

    public List<LockerState> lockerStates = new List<LockerState>();
}
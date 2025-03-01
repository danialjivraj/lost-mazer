using UnityEngine;

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
}

using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int currentLevel;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public bool hasLantern;
    public bool isLanternActive;
}
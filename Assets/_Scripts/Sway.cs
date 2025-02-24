using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swayAmount = 0.5f;
    public float smoothFactor = 2f;

    private Quaternion initialRotation;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float inputX = -Input.GetAxis("Mouse X") * swayAmount;
        float inputY = -Input.GetAxis("Mouse Y") * swayAmount;

        Quaternion targetRotation = Quaternion.Euler(inputY, inputX, 0f) * initialRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothFactor);
    }
}


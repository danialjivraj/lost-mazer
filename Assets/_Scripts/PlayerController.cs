using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // movement and physics
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1.5f;
    public float jumpPower = 6f;
    public float crouchJumpPower = 3f;
    public float gravity = 10f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private bool canMove = true;

    // camera and look
    public Camera playerCam;
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    private float rotationX = 0;
    private float rotationY = 0;

    // crouching
    private bool isCrouching = false;
    private float standingHeight = 2.5f;
    private float crouchHeight = 1.5f;
    private float currentHeight;

    // peeking
    private bool isPeekingLeft = false;
    private bool isPeekingRight = false;
    private Vector3 normalCameraPosition;
    private Vector3 peekLeftPosition = new Vector3(-0.7f, 0, 0);
    private Vector3 peekRightPosition = new Vector3(0.7f, 0, 0);
    private float peekTiltAngle = 30f;
    private Vector3 currentCameraVelocity;
    private Quaternion currentRotationVelocity;

    // zoom
    public int ZoomFOV = 35;
    public int initialFOV;
    public float cameraZoomSmooth = 1;
    private bool isZoomed = false;

    // footstep sounds
    public AudioClip[] woodFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;
    public AudioMixerGroup footstepMixerGroup;
    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;
    private AudioClip[] currentFootstepSounds;

    // breathing sounds
    public AudioSource breathingSlowSource;
    public AudioSource breathingFastSource;
    public float breathingFadeDuration = 1.0f;
    private bool wasRunning = false;

    // hearing range
    public float runningHearingRange = 20f;
    public float walkingHearingRange = 10f;
    public float crouchingHearingRange = 5f;
    public UnityEvent<Vector3, float> OnFootstep;

    // jump and landing
    public AudioSource jumpAudioSource;
    public AudioSource landingAudioSource;
    private bool wasGrounded = true;
    private bool hasJumped = false;
    public float landingHearingVolume = 1f;
    public float landingHearingRange = 12f;
    public float landingGizmoDuration = 1f;
    private float lastLandingTime = -Mathf.Infinity;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentHeight = standingHeight;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        normalCameraPosition = playerCam.transform.localPosition;
        currentFootstepSounds = woodFootstepSounds;

        if (audioSource != null && footstepMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = footstepMixerGroup;
        }

        if (breathingFastSource != null)
        {
            breathingFastSource.loop = true;
            breathingFastSource.volume = 0f;
        }
        if (breathingSlowSource != null)
        {
            breathingSlowSource.loop = false;
            breathingSlowSource.volume = 0f;
        }

        wasGrounded = characterController.isGrounded;

        if (SaveLoadManager.SaveExists())
        {
            var data = SaveLoadManager.LoadGame();
            if (data != null)
            {
                characterController.enabled = false;

                transform.position = data.playerPosition;
                transform.rotation = data.playerRotation;

                characterController.enabled = true;

                rotationY = data.rotationY;
                rotationX = data.rotationX;
                playerCam.transform.localRotation = data.cameraRotation;
                playerCam.fieldOfView = data.cameraFOV;
                isCrouching = data.isCrouching;
                isZoomed = data.isZoomed;
                currentHeight = data.currentHeight;

                Debug.Log("Player state restored from saved game");
            }
        }
        else
        {
            rotationY = transform.rotation.eulerAngles.y;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleCrouching();
        HandleCameraLook();
        HandlePeeking();
        HandleZoom();
        HandleFootsteps();
        HandleBreathingSounds();
        HandleLandingSound();
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        float curSpeedX = canMove ? currentSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? currentSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButtonDown("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = isCrouching ? crouchJumpPower : jumpPower;
            if (!isCrouching && jumpAudioSource != null)
            {
                jumpAudioSource.Play();
            }
            hasJumped = true;
        }

        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCrouching()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref currentCameraVelocity.y, 0.1f);
        characterController.height = currentHeight;
    }

    private void HandleCameraLook()
    {
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    private void HandlePeeking()
    {
        isPeekingLeft = Input.GetKey(KeyCode.Q);
        isPeekingRight = Input.GetKey(KeyCode.E);

        Vector3 targetPosition = normalCameraPosition;
        float targetTilt = 0f;

        Vector3 leftPeekDirection = transform.right * -0.7f;
        Vector3 rightPeekDirection = transform.right * 0.7f;

        bool canPeekLeft = !Physics.Raycast(transform.position, leftPeekDirection, 0.7f);
        bool canPeekRight = !Physics.Raycast(transform.position, rightPeekDirection, 0.7f);

        if (isPeekingLeft && canPeekLeft)
        {
            targetPosition += peekLeftPosition;
            targetTilt = peekTiltAngle;
        }
        else if (isPeekingRight && canPeekRight)
        {
            targetPosition += peekRightPosition;
            targetTilt = -peekTiltAngle;
        }

        playerCam.transform.localPosition = Vector3.SmoothDamp(playerCam.transform.localPosition, targetPosition, ref currentCameraVelocity, 0.1f);
        playerCam.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, Quaternion.Euler(rotationX, 0, targetTilt), Time.deltaTime * 10f);
    }

    private void HandleZoom()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isZoomed = true;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isZoomed = false;
        }

        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, isZoomed ? ZoomFOV : initialFOV, Time.deltaTime * cameraZoomSmooth);
    }

    private void HandleFootsteps()
    {
        bool hasHorizontalInput = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        bool hasVerticalInput = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        bool isGrounded = characterController.isGrounded;

        if (isGrounded && (hasHorizontalInput || hasVerticalInput) && !isCrouching)
        {
            if (!isWalking)
            {
                isWalking = true;
                if (!isFootstepCoroutineRunning)
                {
                    StartCoroutine(PlayFootstepSounds());
                }
            }
        }
        else
        {
            isWalking = false;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void HandleBreathingSounds()
    {
        bool movementInput = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching && movementInput;

        if (isRunning && !wasRunning)
        {
            if (breathingSlowSource != null && breathingSlowSource.isPlaying)
            {
                breathingSlowSource.Stop();
            }
            if (breathingFastSource != null && !breathingFastSource.isPlaying)
            {
                breathingFastSource.volume = 0.5f;
                breathingFastSource.Play();
            }
        }
        else if (!isRunning && wasRunning)
        {
            if (breathingFastSource != null && breathingFastSource.isPlaying)
            {
                breathingFastSource.Stop();
            }
            if (breathingSlowSource != null && !breathingSlowSource.isPlaying)
            {
                breathingSlowSource.volume = 0.5f;
                breathingSlowSource.Play();
            }
        }
        wasRunning = isRunning;
    }

    private void HandleLandingSound()
    {
        if (hasJumped && characterController.isGrounded && !wasGrounded)
        {
            if (!isCrouching)
            {
                if (landingAudioSource != null)
                {
                    landingAudioSource.Play();
                }
                if (OnFootstep != null)
                {
                    OnFootstep.Invoke(transform.position, landingHearingVolume);
                }
                lastLandingTime = Time.time;
            }
            hasJumped = false;
        }
        wasGrounded = characterController.isGrounded;
    }

    IEnumerator PlayFootstepSounds()
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (currentFootstepSounds.Length > 0)
            {
                bool running = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

                int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = currentFootstepSounds[randomIndex];
                audioSource.pitch = running ? 1.5f : 1f;
                audioSource.volume = running ? 1.2f : 0.5f;
                audioSource.Play();

                float volume = running ? 1.2f : 0.5f;
                OnFootstep.Invoke(transform.position, volume);

                yield return new WaitForSeconds(running ? 0.3f : 0.5f);
            }
            else
            {
                yield break;
            }
        }

        isFootstepCoroutineRunning = false;
    }

    public void StopFootsteps()
    {
        isWalking = false;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        StopCoroutine(PlayFootstepSounds());
        isFootstepCoroutineRunning = false;
    }

    public void StopAllMovementAudio()
    {
        StopFootsteps();

        wasRunning = false;

        if (breathingFastSource != null && breathingFastSource.isPlaying)
            breathingFastSource.Stop();

        if (breathingSlowSource != null && breathingSlowSource.isPlaying)
            breathingSlowSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wood"))
        {
            currentFootstepSounds = woodFootstepSounds;
        }
    }

    private void OnDrawGizmos()
    {
        if (isWalking)
        {
            float hearingRange = 0f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                hearingRange = runningHearingRange;
                Gizmos.color = Color.red;
            }
            else if (isCrouching)
            {
                hearingRange = crouchingHearingRange;
                Gizmos.color = Color.green;
            }
            else
            {
                hearingRange = walkingHearingRange;
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawWireSphere(transform.position, hearingRange);
        }
        
        if (Application.isPlaying && Time.time - lastLandingTime < landingGizmoDuration && !isCrouching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, landingHearingRange);
        }
    }

    // getters
    public float RotationX { get { return rotationX; } }
    public float RotationY { get { return rotationY; } }
    public bool IsCrouching { get { return isCrouching; } }
    public bool IsZoomed { get { return isZoomed; } }
    public float CurrentHeight { get { return currentHeight; } }
}
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MultiPlayerController : MonoBehaviourPun
{
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpPower = 6f;
    public float gravity = 10f;

    public Camera playerCam;
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;

    public string pumpkinPrefabName = "Pumpkin";
    public float throwForce = 12f;
    public float spinTorque = 5f;
    [Tooltip("Seconds between throws")]
    public float throwCooldown = 1f;

    private CharacterController cc;
    private Vector3 moveDir = Vector3.zero;
    private float rotX = 0f;
    private float rotY = 0f;
    private float lastThrowTime = -Mathf.Infinity;

    [HideInInspector] public bool IsPaused = false;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (!photonView.IsMine)
        {
            if (playerCam) Destroy(playerCam.gameObject);
        }
    }

    void Start()
    {
        bool isFirst = photonView.OwnerActorNr == 1;
        SetPlayerColor(isFirst ? Color.blue : Color.red);

        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rotY = transform.eulerAngles.y;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        HandleMovement();

        if (!IsPaused)
        {
            HandleLook();

            if (Time.time >= lastThrowTime + throwCooldown &&
                Input.GetButtonDown("Fire1"))
            {
                ThrowPumpkin();
                lastThrowTime = Time.time;
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float inputV = IsPaused ? 0f : Input.GetAxis("Vertical");
        float inputH = IsPaused ? 0f : Input.GetAxis("Horizontal");
        bool  run = !IsPaused && Input.GetKey(KeyCode.LeftShift);

        float speed = run ? runSpeed : walkSpeed;
        moveDir.x = forward.x * inputV * speed + right.x * inputH * speed;
        moveDir.z = forward.z * inputV * speed + right.z * inputH * speed;

        if (cc.isGrounded)
        {
            if (!IsPaused && Input.GetButtonDown("Jump"))
                moveDir.y = jumpPower;
        }
        else
        {
            moveDir.y -= gravity * Time.deltaTime;
        }

        cc.Move(moveDir * Time.deltaTime);
    }

    private void HandleLook()
    {
        rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotX = Mathf.Clamp(rotX, -lookXLimit, lookXLimit);
        rotY += Input.GetAxis("Mouse X") * lookSpeed;

        transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        playerCam.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }

    private void ThrowPumpkin()
    {
        Vector3 spawnPos = playerCam.transform.position + playerCam.transform.forward * 1.2f;
        Quaternion spawnRot = playerCam.transform.rotation;

        GameObject proj = PhotonNetwork.Instantiate(pumpkinPrefabName, spawnPos, spawnRot);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = playerCam.transform.forward * throwForce;
            rb.AddTorque(Random.onUnitSphere * spinTorque, ForceMode.Impulse);
        }

        var projCol = proj.GetComponent<Collider>();
        if (projCol != null)
        {
            foreach (var myCol in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, myCol);
        }
    }

    private void SetPlayerColor(Color c)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.material.color = c;
    }
}

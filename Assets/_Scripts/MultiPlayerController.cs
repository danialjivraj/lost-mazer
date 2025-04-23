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
    public float throwCooldown = 1f;

    public Material eyeMaterial;      
    Transform eyeTransform;     
    Renderer eyeRenderer;

    CharacterController cc;
    Vector3 moveDir = Vector3.zero;
    float rotX, rotY;
    float lastThrowTime = -Mathf.Infinity;
    [HideInInspector] public bool IsPaused;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        eyeTransform = transform.Find("Eye");
        if (eyeTransform != null)
            eyeRenderer  = eyeTransform.GetComponent<Renderer>();

        if (!photonView.IsMine && playerCam) 
            Destroy(playerCam.gameObject);

        if (photonView.IsMine && eyeTransform != null)
            eyeTransform.gameObject.SetActive(false);
    }

    void Start()
    {
        object raw;
        photonView.Owner.CustomProperties.TryGetValue("Team", out raw);
        int team = raw != null ? (int)raw : 0;
        Color c = team == 0 ? Color.blue : Color.red;

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            if (r == eyeRenderer) 
                continue;
            r.material.color = c;
        }

        if (eyeRenderer != null)
        {
            eyeRenderer.material = eyeMaterial;
        }

        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rotY = transform.eulerAngles.y;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (MultiplayerScoreManager.Instance.IsRoundOver) return;
        HandleMovement();
        if (!IsPaused)
        {
            HandleLook();
            if (Time.time >= lastThrowTime + throwCooldown && Input.GetButtonDown("Fire1"))
            {
                ThrowPumpkin();
                lastThrowTime = Time.time;
            }
        }
    }

    void HandleMovement()
    {
        Vector3 f = transform.TransformDirection(Vector3.forward);
        Vector3 r = transform.TransformDirection(Vector3.right);
        float v = IsPaused ? 0 : Input.GetAxis("Vertical");
        float h = IsPaused ? 0 : Input.GetAxis("Horizontal");
        bool run = !IsPaused && Input.GetKey(KeyCode.LeftShift);
        float sp = run ? runSpeed : walkSpeed;
        moveDir.x = f.x * v * sp + r.x * h * sp;
        moveDir.z = f.z * v * sp + r.z * h * sp;
        if (cc.isGrounded)
        {
            if (!IsPaused && Input.GetButtonDown("Jump"))
                moveDir.y = jumpPower;
        }
        else moveDir.y -= gravity * Time.deltaTime;
        cc.Move(moveDir * Time.deltaTime);
    }

    void HandleLook()
    {
        rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotX = Mathf.Clamp(rotX, -lookXLimit, lookXLimit);
        rotY += Input.GetAxis("Mouse X") * lookSpeed;
        transform.rotation = Quaternion.Euler(0, rotY, 0);
        playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
    }

    void ThrowPumpkin()
    {
        Vector3 pos = playerCam.transform.position + playerCam.transform.forward * 1.2f;
        Quaternion rot = playerCam.transform.rotation;
        var proj = PhotonNetwork.Instantiate(pumpkinPrefabName, pos, rot);
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = playerCam.transform.forward * throwForce;
            rb.AddTorque(Random.onUnitSphere * spinTorque, ForceMode.Impulse);
        }
        var col = proj.GetComponent<Collider>();
        if (col != null)
            foreach (var my in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(col, my);
    }

    public void ResetPosition(Transform spawn)
    {
        cc.enabled = false;
        transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        cc.enabled = true;
        moveDir = Vector3.zero;
    }
}

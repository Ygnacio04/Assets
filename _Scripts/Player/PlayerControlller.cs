using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    public enum PlayerMode
    {
        Player,
        Turret
    }
    [Header("Mode")]
    public PlayerMode mode = PlayerMode.Turret;
    [SerializeField, Range(1, 5)] private float moveSpeed = 2f;
    private float runSpeed => moveSpeed * 4;
    private float speedMultiplier = 1.0f;
    [Header("Rotation Settings")]
    [SerializeField, Range(20, 360)] private float rotationSpeed = 180f;
    [SerializeField] private float rotationAngleXLimit = 60f;
    [SerializeField] private bool isXInverted = false;
    [SerializeField] private bool isYInverted = false;

    [Header("Turret Settings")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    private float nextFireTime;

    [SerializeField] private Transform cameraTransform;

    private int _inversionX => isXInverted ? -1 : 1;
    private int _inversionY => isYInverted ? -1 : 1;

    private CharacterController _characterController;
    private Vector2 _cameraRotation;

    private float verticalVelocity;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float terminalVelocity = 20f;
    [SerializeField] private float jumpForce = 5f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _characterController = GetComponent<CharacterController>();
        _cameraRotation = Vector2.zero;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleRotation();

        if (mode == PlayerMode.Player)
        {
            HandleMovement();
        }
        else if (mode == PlayerMode.Turret)
        {
            HandleShooting();
        }
    }

    private void HandleRotation()
    {
        _cameraRotation.x = Input.GetAxis("Mouse Y") * _inversionY;
        _cameraRotation.y = Input.GetAxis("Mouse X") * _inversionX;

        transform.Rotate(0, _cameraRotation.y * rotationSpeed * Time.deltaTime, 0);

        Vector3 rotationVector = new Vector3(_cameraRotation.x, 0, 0);
        cameraTransform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);

        float angleTransformation = cameraTransform.rotation.eulerAngles.x > 180
            ? cameraTransform.localEulerAngles.x - 360
            : cameraTransform.localEulerAngles.x;

        rotationVector = new Vector3(Mathf.Clamp(angleTransformation, -rotationAngleXLimit, rotationAngleXLimit), cameraTransform.localEulerAngles.y, 0);
        cameraTransform.localEulerAngles = rotationVector;
    }

    private void HandleTurretRotation()
    {
        _cameraRotation.x = Input.GetAxis("Mouse Y") * _inversionY;
        _cameraRotation.y = Input.GetAxis("Mouse X") * _inversionX;

        transform.Rotate(0, _cameraRotation.y * rotationSpeed * Time.deltaTime, 0);

        Vector3 rotationVector = new Vector3(_cameraRotation.x, 0, 0);
        cameraTransform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);

        float angleTransformation = cameraTransform.rotation.eulerAngles.x > 180
            ? cameraTransform.localEulerAngles.x - 360
            : cameraTransform.localEulerAngles.x;

        rotationVector = new Vector3(Mathf.Clamp(angleTransformation, -rotationAngleXLimit, rotationAngleXLimit), cameraTransform.localEulerAngles.y, 0);
        cameraTransform.localEulerAngles = rotationVector;
    }

    private void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = (isRunning ? runSpeed : moveSpeed) * speedMultiplier;

        if (!_characterController.isGrounded)
        {
            ApplyGravity();
        }
        else
        {
            if (Input.GetButton("Jump"))
            {
                verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity = -1f;
            }
        }

        Vector3 moveDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        moveDirection.Normalize();

        _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime) + Vector3.up * (verticalVelocity * Time.deltaTime));
    }

    private void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity <= -terminalVelocity)
        {
            verticalVelocity = -terminalVelocity;
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (projectilePrefab != null && firePoint != null)
            {
                Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                Debug.LogWarning("Missing firePoint or projectilePrefab in Turret mode.");
            }
        }
    }
    public void SetSpeedMultiplier(float multiplier) => speedMultiplier = multiplier;
    public void ResetSpeed() => speedMultiplier = 1.0f;
}
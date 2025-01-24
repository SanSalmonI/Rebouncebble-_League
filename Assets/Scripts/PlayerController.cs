using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform mainCamera;
    private float turnSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Get camera reference safely
        mainCamera = Camera.main?.transform;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not found! Creating a reference to current camera.");
            mainCamera = Object.FindAnyObjectByType<Camera>().transform;
        }
    }

    void Update()
    {
        GroundCheck();
        HandleMovement();

        // Only jump with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            HandleJump();
        }

        ApplyGravity();
    }

    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal"); // Changed from GetAxisRaw for smoother movement
        float vertical = Input.GetAxis("Vertical");

        // Create movement vector
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && mainCamera != null)
        {
            // Calculate movement angle based on camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 1f / rotationSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move in the correct direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 movement = moveDir * moveSpeed * Time.deltaTime;
            controller.Move(movement);
        }
    }

    void HandleJump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
    }
}

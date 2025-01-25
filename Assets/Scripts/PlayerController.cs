using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private CharacterController controller;
    private Transform mainCamera;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private float currentSpeed;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main?.transform ?? FindFirstObjectByType<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckGrounded();
        HandleRotation();
        HandleMovement();
        HandleJumping();
        ApplyGravity();
    }

    void HandleRotation()
    {
        float rotationInput = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotationInput);
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        float targetSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        Vector3 moveDirection = transform.forward * vertical;
        moveDirection.Normalize();

        if (moveDirection.magnitude > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        Vector3 targetVelocity = moveDirection * currentSpeed;
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, acceleration * Time.deltaTime);
        controller.Move(currentMoveVelocity * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

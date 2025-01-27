using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = -30f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("References")]
    [SerializeField] private Transform orientation;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // Input fields
    [HideInInspector] public Vector3 inputDirection;
    [HideInInspector] public float rotationInput;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isSprinting;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGround();
        HandleMovement();
        HandleRotation();
        HandleJump();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        Vector3 moveDirection = orientation.forward * inputDirection.z;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);
        orientation.rotation = transform.rotation;
    }

    private void HandleJump()
    {
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize ground check sphere in editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

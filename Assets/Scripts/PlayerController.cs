using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 6f;
    [SerializeField] protected float sprintMultiplier = 2f;
    [SerializeField] protected float rotationSpeed = 10f;
    [SerializeField] protected float jumpHeight = 2f;
    [SerializeField] protected float gravity = -9.81f;
    [SerializeField] protected float acceleration = 10f;
    [SerializeField] protected float deceleration = 8f;

    [Header("Ground Detection")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float groundCheckRadius = 0.2f;

    protected CharacterController controller;
    protected Transform mainCamera;
    protected Vector3 velocity;
    protected Vector3 currentMoveVelocity;
    protected float currentSpeed;
    protected bool isGrounded;
    protected Vector3 inputDirection;
    protected float rotationInput;
    protected bool isJumping;
    protected bool isSprinting;

    protected virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main?.transform;

        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = Vector3.down * 0.5f;
            groundCheck = check.transform;
        }
    }

    protected virtual void Update()
    {
        CheckGrounded();
        GetInput();
        HandleRotation();
        HandleMovement();
        HandleJumping();
        ApplyGravity();
    }

    protected virtual void GetInput()
    {
        if (Input.GetKey(KeyCode.W)) inputDirection.z = 1;
        else if (Input.GetKey(KeyCode.S)) inputDirection.z = -1;
        else inputDirection.z = 0;

        if (Input.GetKey(KeyCode.D)) rotationInput = 1;
        else if (Input.GetKey(KeyCode.A)) rotationInput = -1;
        else rotationInput = 0;

        isJumping = Input.GetKey(KeyCode.Space);
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    protected virtual void HandleRotation()
    {
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotation);
    }

    protected virtual void HandleMovement()
    {
        float targetSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        Vector3 moveDirection = transform.forward * inputDirection.z;
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

    protected virtual void HandleJumping()
    {
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    protected virtual void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    protected virtual void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

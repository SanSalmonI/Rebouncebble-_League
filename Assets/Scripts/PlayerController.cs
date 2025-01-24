using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 7f;

    [Header("References")]
    public Transform orientation; // Gets orientation from ThirdPersonCam
    private Rigidbody rb;
    private ThirdPersonCam cameraScript;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find the ThirdPersonCam script and assign its orientation
        cameraScript = FindObjectOfType<ThirdPersonCam>();
        if (cameraScript != null)
        {
            orientation = cameraScript.orientation;
        }
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        if (orientation == null) return; // Ensure camera is available

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Movement relative to camera orientation
        Vector3 moveDirection = orientation.forward * moveZ + orientation.right * moveX;
        moveDirection.y = 0; // Ignore vertical movement for better control

        if (moveDirection.magnitude > 0.1f)
        {
            // Move player using Rigidbody for smooth physics-based movement
            rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.deltaTime);

            // Smoothly rotate player to face movement direction
            transform.forward = Vector3.Slerp(transform.forward, moveDirection.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

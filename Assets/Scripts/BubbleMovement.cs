using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Bounce Physics")]
    [SerializeField] private float groundBounceForce = 45f;
    [SerializeField] private float playerImpactForce = 25f;
    [SerializeField] private float playerJumpMultiplier = 4f;
    [SerializeField] private float maxBounceVelocity = 40f;
    [SerializeField] private float horizontalBounceFactor = 0.8f;
    [SerializeField] private float bounceDamping = 0.98f;
    [SerializeField] private float upwardBias = 1.5f;

    [Header("Movement")]
    // Negative gravity for downward force, or rename and invert sign:
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float airResistance = 0.997f;
    [SerializeField] private float floatTime = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private float squishAmount = 0.3f;
    [SerializeField] private float squishRecoverySpeed = 8f;

    [Header("Size Changes")]
    [SerializeField] private float startSizeXYZ = 10f;
    [SerializeField] private float sizeTimer = 40f;
    [SerializeField] private float minSize = 0.1f;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial; // Default
    [SerializeField] private Material player1Material; // Red
    [SerializeField] private Material player2Material; // Blue

    private Vector3 currentBaseScale;
    private float elapsedTime = 0f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isSquished;
    private float lastBounceTime;

    private Renderer ballRenderer; // For material change
    private int hitCount = 0; // Tracks the number of hits

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>(); // Get the Renderer to change materials

        SetupRigidbody();

        originalScale = new Vector3(startSizeXYZ, startSizeXYZ, startSizeXYZ);
        transform.localScale = originalScale;
        currentBaseScale = originalScale;

        // Set initial material (for player 1, Red)
        ballRenderer.material = defaultMaterial;
    }

    void SetupRigidbody()
    {
        // If you want to apply custom gravity, set this to false:
        rb.useGravity = false;
        // Unity standard drag properties:
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.8f;
        rb.mass = 0.8f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyAirResistance();
        ApplyFloating();
        DecreaseSize();
        RecoverFromSquish();
        
    }

    void DecreaseSize()
    {
        if (elapsedTime < sizeTimer)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = Mathf.Lerp(1f, minSize / startSizeXYZ, elapsedTime / sizeTimer);

            Vector3 newScale = new Vector3(startSizeXYZ, startSizeXYZ, startSizeXYZ) * scaleMultiplier;

            // Ensure no dimension goes below minSize
            newScale.x = Mathf.Max(newScale.x, minSize);
            newScale.y = Mathf.Max(newScale.y, minSize);
            newScale.z = Mathf.Max(newScale.z, minSize);

            currentBaseScale = newScale;
        }
    }

    void ApplyGravity()
    {
        // Only apply our custom gravity if we have "floated" long enough and are not on the ground
        if (Time.time - lastBounceTime > floatTime && !IsTouchingGround())
        {
            // gravity is negative, so Vector3.up * negative => downward force
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    bool IsTouchingGround()
    {
        // If you want more robust ground checks, consider SphereCast
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground"));
    }

    void ApplyAirResistance()
    {
        // Must be rb.velocity in standard Unity
        rb.linearVelocity *= airResistance;
    }

    void ApplyFloating()
    {
        // A small upward push if the velocity is downward
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Acceleration);
        }
    }

    void HandlePlayerCollision(Collision collision)
    {
        // If you are using the default CharacterController, it has no velocity property
        // This will fail unless you have a custom CharacterController or a different approach
        // For demonstration, let's just use the collision's relative velocity:
        Vector3 playerVelocity = collision.relativeVelocity;

        float impactSpeed = collision.relativeVelocity.magnitude;
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;

        // Start from base
        float bounceForce = playerImpactForce;

        // Check if the "player" is moving upward relative to the bubble
        if (playerVelocity.y > 0)
        {
            bounceForce *= playerJumpMultiplier;

            // Also check angle
            float verticalAngle = Vector3.Angle(Vector3.up, hitDirection);
            if (verticalAngle < 45f)
            {
                // Hitting from below
                bounceForce *= 1.5f;
            }
        }

        Vector3 bounceDirection = (hitDirection + Vector3.up * upwardBias).normalized;

        float impactVelocity = Mathf.Abs(rb.linearVelocity.y);
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Use updated bounceForce
        Vector3 bounceVelocity = currentHorizontalVelocity * bounceDamping
                               + bounceDirection * Mathf.Sqrt(2f * bounceForce * (impactVelocity + 5f));

        // Ensure Y is not below a minimum
        float minY = Mathf.Sqrt(2f * bounceForce * (impactSpeed + 5f));
        bounceVelocity.y = Mathf.Max(bounceVelocity.y, minY);

        rb.linearVelocity = Vector3.ClampMagnitude(bounceVelocity, maxBounceVelocity);
        lastBounceTime = Time.time;
    }

    void HandleGroundBounce(Collision collision)
    {
        float impactVelocity = Mathf.Abs(rb.linearVelocity.y);
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        Vector3 bounceVelocity = currentHorizontalVelocity * bounceDamping
                               + Vector3.up * Mathf.Sqrt(2f * groundBounceForce * (impactVelocity + 5f));

        rb.linearVelocity = bounceVelocity;
        lastBounceTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bubble collided with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);

            // Player 1 (tag "Player") hits the ball, switch to Red
            ballRenderer.material = player1Material; // Switch to Blue for Player 2     
        }
        else if (collision.gameObject.CompareTag("Player2"))
        {
            HandlePlayerCollision(collision);

            // Player 2 (tag "Player2") hits the ball, switch to Blue
            if (collision.gameObject.CompareTag("Player2"))
            {
                    ballRenderer.material = player2Material; // Switch to Blue for Player 2
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            HandleGroundBounce(collision);
        }

        ApplySquishEffect();
    }

    void ApplySquishEffect()
    {
        // Squish relative to the *current* base scale
        Vector3 squishScale = currentBaseScale;
        squishScale.y *= (1f - squishAmount);
        squishScale.x *= (1f + squishAmount * 0.5f);
        squishScale.z *= (1f + squishAmount * 0.5f);

        transform.localScale = squishScale;
        isSquished = true;
    }

    void RecoverFromSquish()
    {
        if (isSquished)
        {
            // Smoothly move from current squish scale back to the latest "currentBaseScale"
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                currentBaseScale,
                Time.deltaTime * squishRecoverySpeed
            );

            // If nearly at the base scale, snap fully and stop squishing
            if (Vector3.Distance(transform.localScale, currentBaseScale) < 0.01f)
            {
                transform.localScale = currentBaseScale;
                isSquished = false;
            }
        }
        else
        {
            // If not squished, make sure we match the updated base scale
            transform.localScale = currentBaseScale;
        }
    }

}

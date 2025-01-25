using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Bounce Physics")]
    [SerializeField] private float groundBounceForce = 35f; // Massive upward force
    [SerializeField] private float playerImpactForce = 15f;
    [SerializeField] private float playerJumpMultiplier = 3f;
    [SerializeField] private float maxBounceVelocity = 30f; // Increased max velocity
    [SerializeField] private float horizontalBounceFactor = 1.2f;
    [SerializeField] private float bounceDamping = 0.95f; // Higher value to maintain bounce height

    [Header("Movement")]
    [SerializeField] private float gravity = -20f; // Stronger gravity for faster descent
    [SerializeField] private float airResistance = 0.995f; // Reduced air resistance

    [Header("Visual Effects")]
    [SerializeField] private float squishAmount = -15;
    [SerializeField] private float squishRecoverySpeed = 0.99f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isSquished;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupRigidbody();
        originalScale = transform.localScale;
    }

    void SetupRigidbody()
    {
        rb.useGravity = true;
        rb.linearDamping = 0.2f;
        rb.angularDamping = 0.8f;
        rb.mass = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyAirResistance();
        RecoverFromSquish();
    }

    void ApplyGravity()
    {
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }

    void ApplyAirResistance()
    {
        rb.linearVelocity *= airResistance;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            HandleGroundBounce(collision);
        }

        ApplySquishEffect();
    }

    void HandlePlayerCollision(Collision collision)
    {
        CharacterController playerController = collision.gameObject.GetComponent<CharacterController>();
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;
        Vector3 playerVelocity = playerController ? playerController.velocity : collision.relativeVelocity;
        float impactSpeed = collision.relativeVelocity.magnitude;

        // Calculate bounce direction and force
        Vector3 bounceDirection;
        float bounceForce = playerImpactForce;

        // Player jumping into bubble
        if (playerVelocity.y > 0)
        {
            bounceForce *= playerJumpMultiplier;
            Vector3 horizontalDir = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;
            bounceDirection = (horizontalDir * horizontalBounceFactor + Vector3.up).normalized;
        }
        // Player running into bubble
        else
        {
            bounceDirection = (hitDirection + Vector3.up * 0.5f).normalized;
        }

        // Apply the bounce
        Vector3 bounceVector = bounceDirection * bounceForce * (1 + impactSpeed * 0.2f);
        rb.linearVelocity = Vector3.ClampMagnitude(bounceVector, maxBounceVelocity);
    }

    void HandleGroundBounce(Collision collision)
    {
        float impactVelocity = Mathf.Abs(rb.linearVelocity.y);
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Maintain horizontal momentum while bouncing up
        Vector3 bounceVelocity = currentHorizontalVelocity * bounceDamping +
                                Vector3.up * Mathf.Sqrt(2f * groundBounceForce * impactVelocity);

        rb.linearVelocity = bounceVelocity;
    }

    void ApplySquishEffect()
    {
        Vector3 squishScale = originalScale;
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
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * squishRecoverySpeed);
            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                transform.localScale = originalScale;
                isSquished = false;
            }
        }
    }
}

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
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float airResistance = 0.997f;
    [SerializeField] private float floatTime = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private float squishAmount = 0.3f;
    [SerializeField] private float squishRecoverySpeed = 8f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isSquished;
    private float lastBounceTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupRigidbody();
        originalScale = transform.localScale;
    }

    void SetupRigidbody()
    {
        rb.useGravity = true;
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
        RecoverFromSquish();
    }

    void ApplyGravity()
    {
        if (Time.time - lastBounceTime > floatTime)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    void ApplyAirResistance()
    {
        rb.linearVelocity *= airResistance;
    }

    void ApplyFloating()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Acceleration);
        }
    }

    void HandlePlayerCollision(Collision collision)
    {
        CharacterController playerController = collision.gameObject.GetComponent<CharacterController>();
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;
        Vector3 playerVelocity = playerController ? playerController.velocity : collision.relativeVelocity;
        float impactSpeed = collision.relativeVelocity.magnitude;

        Vector3 bounceDirection;
        float bounceForce = playerImpactForce;

        // Calculate vertical angle between player and bubble
        float verticalAngle = Vector3.Angle(Vector3.up, hitDirection);

        if (playerVelocity.y > 0)
        {
            bounceForce *= playerJumpMultiplier;

            if (verticalAngle < 45f) // Player hitting from below
            {
                bounceDirection = Vector3.up;
                bounceForce *= 1.5f;
            }
            else // Player hitting from an angle
            {
                Vector3 horizontalDir = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;
                bounceDirection = (horizontalDir * horizontalBounceFactor + Vector3.up * upwardBias).normalized;
            }
        }
        else
        {
            bounceDirection = (hitDirection + Vector3.up * upwardBias).normalized;
        }

        Vector3 bounceVector = bounceDirection * bounceForce * (1 + impactSpeed * 0.3f);
        rb.linearVelocity = Vector3.ClampMagnitude(bounceVector, maxBounceVelocity);
        lastBounceTime = Time.time;
    }

    void HandleGroundBounce(Collision collision)
    {
        float impactVelocity = Mathf.Abs(rb.linearVelocity.y);
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        Vector3 bounceVelocity = currentHorizontalVelocity * bounceDamping +
                                Vector3.up * Mathf.Sqrt(2f * groundBounceForce * (impactVelocity + 5f));

        rb.linearVelocity = bounceVelocity;
        lastBounceTime = Time.time;
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

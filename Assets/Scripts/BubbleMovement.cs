using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Bubble Physics")]
    [SerializeField] private float floatForce = 5f;
    [SerializeField] private float swayAmount = 0.5f;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float bounciness = 0.8f;
    [SerializeField] private float maxBounceForce = 20f;
    [SerializeField] private float minBounceForce = 5f;
    [SerializeField] private float maxHeight = 20f;
    [SerializeField] private float randomMovementIntensity = 0.8f;

    [Header("Visual Feedback")]
    [SerializeField] private float squishAmount = 0.3f;
    [SerializeField] private float squishRecoverySpeed = 5f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 originalScale;
    private bool isSquished;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupRigidbody();
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    void SetupRigidbody()
    {
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.mass = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        ApplyFloatForce();
        ApplySway();
        AddRandomMovement();
        ClampHeight();
        RecoverFromSquish();
    }

    void ApplyFloatForce()
    {
        float heightFactor = 1f - (transform.position.y / maxHeight);
        rb.AddForce(Vector3.up * floatForce * heightFactor);
    }

    void ApplySway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAmount;
        Vector3 sway = new Vector3(swayX, 0, swayZ);
        rb.AddForce(sway, ForceMode.Force);
    }

    void AddRandomMovement()
    {
        if (Random.value < 0.1f)
        {
            Vector3 randomForce = Random.insideUnitSphere * randomMovementIntensity;
            rb.AddForce(randomForce, ForceMode.Impulse);
        }
    }

    void ClampHeight()
    {
        if (transform.position.y > maxHeight)
        {
            Vector3 pos = transform.position;
            pos.y = maxHeight;
            transform.position = pos;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 0), rb.linearVelocity.z);
        }
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

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
        ApplySquishEffect(collision);
    }

    void HandleCollision(Collision collision)
    {
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;
        float impactForce = collision.relativeVelocity.magnitude;

        Vector3 bounceDirection = Vector3.Reflect(collision.relativeVelocity.normalized, hitDirection);
        float bounceForce = Mathf.Clamp(impactForce * bounciness, minBounceForce, maxBounceForce);

        rb.linearVelocity = bounceDirection * bounceForce;
        rb.AddTorque(Random.insideUnitSphere * bounceForce, ForceMode.Impulse);
    }

    void ApplySquishEffect(Collision collision)
    {
        Vector3 squishScale = originalScale;
        squishScale.y *= (1f - squishAmount);
        squishScale.x *= (1f + squishAmount * 0.5f);
        squishScale.z *= (1f + squishAmount * 0.5f);
        transform.localScale = squishScale;
        isSquished = true;
    }
}

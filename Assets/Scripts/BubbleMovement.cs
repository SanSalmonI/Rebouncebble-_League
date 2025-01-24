using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Bubble Physics")]
    [SerializeField] private float floatForce = 2f;
    [SerializeField] private float descentSpeed = 1f;
    [SerializeField] private float playerBounceForce = 10f;
    [SerializeField] private float playerJumpBounceMultiplier = 2.5f;
    [SerializeField] private float maxBounceVelocity = 15f;
    [SerializeField] private float airResistance = 0.98f;
    [SerializeField] private float minBounceForce = 5f;

    [Header("Visual Effects")]
    [SerializeField] private float wobbleAmount = 0.3f;
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float squishAmount = 0.3f;
    [SerializeField] private float squishRecoverySpeed = 5f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 originalScale;
    private bool isSquished;
    private bool gameOver = false;

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
        rb.linearDamping = 1f;
        rb.angularDamping = 0.5f;
        rb.mass = 0.1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        if (!gameOver)
        {
            ApplyFloatingEffect();
            ApplyWobble();
            ApplyAirResistance();
            RecoverFromSquish();
        }
    }

    void ApplyFloatingEffect()
    {
        Vector3 descent = Vector3.down * descentSpeed;
        rb.AddForce(descent + Vector3.up * floatForce);
    }

    void ApplyWobble()
    {
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed) * wobbleAmount;
        rb.AddForce(new Vector3(wobbleX, 0, wobbleZ), ForceMode.Force);
    }

    void ApplyAirResistance()
    {
        rb.linearVelocity *= airResistance;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
            ApplySquishEffect();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            GameOver();
        }
    }

    void HandlePlayerCollision(Collision collision)
    {
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;
        float impactSpeed = collision.relativeVelocity.magnitude;

        CharacterController playerController = collision.gameObject.GetComponent<CharacterController>();
        float upwardVelocity = playerController ? playerController.velocity.y : 0;
        float bounceForce = playerBounceForce;

        if (upwardVelocity > 0)
        {
            bounceForce *= playerJumpBounceMultiplier;
            hitDirection = (hitDirection + Vector3.up).normalized;
        }

        bounceForce = Mathf.Max(bounceForce, minBounceForce);
        Vector3 bounceVector = hitDirection * bounceForce * (1 + impactSpeed / 10);
        rb.linearVelocity = Vector3.ClampMagnitude(bounceVector, maxBounceVelocity);
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

    void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over - Bubble touched the ground!");
        // Add your game over logic here
    }

    void OnDrawGizmos()
    {
        // Visual debug for bubble behavior
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
    }
}

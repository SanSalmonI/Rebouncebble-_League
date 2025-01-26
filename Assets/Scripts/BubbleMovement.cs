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

    [Header("Size Changes")]
    [SerializeField] private float startSizeXYZ = 10f;
    [SerializeField] private float sizeTimer = 40f;
    [SerializeField] private float minSize = 0.1f;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial; // Default
    [SerializeField] private Material player1Material; // Red
    [SerializeField] private Material player2Material; // Blue

    [SerializeField] private float currentHotAirForce = 0f; // Tracks the current upward force
    [SerializeField] private float maxHotAirForce = 30f;   // Maximum upward force
    [SerializeField] private float gradualRiseSpeed = 3f; // Speed of force increment
    [SerializeField] private float gradualFallSpeed = 1.5f; // Speed of force decrement

    private Vector3 currentBaseScale;
    private float elapsedTime = 0f;
    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isSquished;
    private float lastBounceTime;
    private Renderer ballRenderer;
    private bool hitSpikes = false;
    private float spikeTimer = 0f;
    private float spikeCooldawDuration = 2f;
    public bool spawnBubble = false;

    public LifeManager lifeManager;

 
    private enum LastHitter { None, Red, Blue }
    private LastHitter lastHitter = LastHitter.None;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();

        SetupRigidbody();

        originalScale = new Vector3(startSizeXYZ, startSizeXYZ, startSizeXYZ);
        transform.localScale = originalScale;
        currentBaseScale = originalScale;

        ballRenderer.material = defaultMaterial; // Start with a default
    }

    void SetupRigidbody()
    {
        rb.useGravity = false;
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
        resetCurrentAirForce();
        resetSpikesTimer();
    }

    void DecreaseSize()
    {
        if (elapsedTime < sizeTimer)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = Mathf.Lerp(1f, minSize / startSizeXYZ, elapsedTime / sizeTimer);

            Vector3 newScale = new Vector3(startSizeXYZ, startSizeXYZ, startSizeXYZ) * scaleMultiplier;
            newScale.x = Mathf.Max(newScale.x, minSize);
            newScale.y = Mathf.Max(newScale.y, minSize);
            newScale.z = Mathf.Max(newScale.z, minSize);

            currentBaseScale = newScale;
        }
    }

    void ApplyGravity()
    {
        if (Time.time - lastBounceTime > floatTime && !IsTouchingGround())
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    bool IsTouchingGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground"));
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
        Vector3 playerVelocity = collision.relativeVelocity;
        float impactSpeed = collision.relativeVelocity.magnitude;
        Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;

        float bounceForce = playerImpactForce;

        if (playerVelocity.y > 0)
        {
            bounceForce *= playerJumpMultiplier;
            float verticalAngle = Vector3.Angle(Vector3.up, hitDirection);
            if (verticalAngle < 45f)
            {
                bounceForce *= 1.5f;
            }
        }

        Vector3 bounceDirection = (hitDirection + Vector3.up * upwardBias).normalized;
        float impactVelocity = Mathf.Abs(rb.linearVelocity.y);
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        Vector3 bounceVelocity = currentHorizontalVelocity * bounceDamping
                                 + bounceDirection * Mathf.Sqrt(2f * bounceForce * (impactVelocity + 5f));

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

    void HandleSpikesBounce(Collision collision)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;
        Vector3 bounceDirection = (collisionNormal + Vector3.up).normalized;

        float horizontalBounceForce = 50f;
        float verticalBounceForce = 30f;

        Vector3 horizontalForce = new Vector3(bounceDirection.x, 0, bounceDirection.z) * horizontalBounceForce;
        Vector3 verticalForce = Vector3.up * verticalBounceForce;

        rb.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);

        Vector3 newScale = transform.localScale * 0.5f;
        newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
        transform.localScale = newScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bubble collided with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
            // The bubble is now Red
            lastHitter = LastHitter.Red;
            ballRenderer.material = player1Material;
        }
        else if (collision.gameObject.CompareTag("Player2"))
        {
            HandlePlayerCollision(collision);
            // The bubble is now Blue
            lastHitter = LastHitter.Blue;
            ballRenderer.material = player2Material;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            HandleGroundBounce(collision);

          
            // SUBTRACT A LIFE FROM THE PLAYER WHO LAST HIT THE BUBBLE
            if (lastHitter == LastHitter.Red)
            {
                lifeManager.LoseRedLife();
            }
            else if (lastHitter == LastHitter.Blue)
            {
                lifeManager.LoseBlueLife();
            }
            // Reset ownership 
            lastHitter = LastHitter.None;
            
        }

        ApplySquishEffect();

        if (collision.gameObject.CompareTag("Spike"))
        {
            Debug.Log("Bubble hit spikes!");
            hitSpikes = true;
            HandleSpikesBounce(collision);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            Debug.Log("Bubble is in fire trigger!");
            if (currentHotAirForce < maxHotAirForce)
            {
                currentHotAirForce += gradualRiseSpeed * Time.deltaTime;
            }
            rb.AddForce(Vector3.up * currentHotAirForce, ForceMode.Acceleration);
        }
    }

    void resetCurrentAirForce()
    {
        if (currentHotAirForce > 0)
        {
            currentHotAirForce -= gradualFallSpeed * Time.deltaTime;
        }
    }

    void resetSpikesTimer()
    {
        if (hitSpikes)
        {
            spikeTimer += Time.deltaTime;
            if (spikeTimer >= spikeCooldawDuration)
            {
                hitSpikes = false;
                spikeTimer = 0f;
            }
        }
    }

    void ApplySquishEffect()
    {
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
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                currentBaseScale,
                Time.deltaTime * squishRecoverySpeed
            );

            if (Vector3.Distance(transform.localScale, currentBaseScale) < 0.01f)
            {
                transform.localScale = currentBaseScale;
                isSquished = false;
            }
        }
        else
        {
            transform.localScale = currentBaseScale;
        }
    }
}

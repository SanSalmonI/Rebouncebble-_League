using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [SerializeField] private float floatForce = 5f;
    [SerializeField] private float swayAmount = 0.5f;
    [SerializeField] private float swaySpeed = 2f;

    private Rigidbody rb;
    private Vector3 startPosition;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 2f;
        startPosition = transform.position;

    }

    void FixedUpdate()
    {
        // Add buoyancy force
        rb.AddForce(Vector3.up * floatForce);

        // Add gentle horizontal sway
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

        Vector3 sway = new Vector3(swayX, 0, swayZ);
        rb.AddForce(sway);

        // Add slight random movement
        if (Random.value < 0.1f)
        {
            Vector3 randomForce = Random.insideUnitSphere * 0.5f;
            rb.AddForce(randomForce, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Add bounce effect
        Vector3 bounceForce = Vector3.up * floatForce * 2f;
        rb.AddForce(bounceForce, ForceMode.Impulse);
    }
}

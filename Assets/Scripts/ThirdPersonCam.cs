using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionMask;

    private float currentRotationX;
    private float currentRotationY;
    private float currentDistance;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        currentDistance = distance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        currentRotationY += mouseX;
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance = Mathf.Clamp(currentDistance - scroll * 5f, minDistance, maxDistance);
    }

    void UpdateCameraPosition()
    {
        Vector3 direction = new Vector3(0, 0, -currentDistance);
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;

        Vector3 desiredPosition = targetPosition + rotation * direction;

        if (Physics.SphereCast(targetPosition, collisionRadius, desiredPosition - targetPosition,
            out RaycastHit hit, currentDistance, collisionMask))
        {
            desiredPosition = targetPosition + (desiredPosition - targetPosition).normalized * (hit.distance - 0.1f);
        }

        transform.position = desiredPosition;
        transform.LookAt(targetPosition);
    }
}

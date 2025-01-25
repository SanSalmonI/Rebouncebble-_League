using UnityEngine;

public class SplitScreenCamera : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform player1Target;
    [SerializeField] private Transform player2Target;

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

    private Camera camera1;
    private Camera camera2;
    private float currentRotationX1, currentRotationX2;
    private float currentRotationY1, currentRotationY2;
    private float currentDistance1, currentDistance2;

    void Start()
    {
        SetupCameras();
        currentDistance1 = currentDistance2 = distance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetupCameras()
    {
        // Setup Player 1 Camera (Left Side)
        camera1 = gameObject.AddComponent<Camera>();
        camera1.rect = new Rect(0, 0, 0.5f, 1);
        camera1.depth = 0;

        // Setup Player 2 Camera (Right Side)
        GameObject cam2Obj = new GameObject("Player2Camera");
        camera2 = cam2Obj.AddComponent<Camera>();
        camera2.rect = new Rect(0.5f, 0, 0.5f, 1);
        camera2.depth = 0;
    }

    void LateUpdate()
    {
        if (!player1Target || !player2Target) return;

        // Update Player 1 Camera
        UpdateCamera(camera1, player1Target, ref currentRotationX1, ref currentRotationY1, ref currentDistance1, true);

        // Update Player 2 Camera
        UpdateCamera(camera2, player2Target, ref currentRotationX2, ref currentRotationY2, ref currentDistance2, false);
    }

    void UpdateCamera(Camera cam, Transform target, ref float rotX, ref float rotY, ref float dist, bool isPlayer1)
    {
        // Handle rotation based on player
        HandleRotation(ref rotX, ref rotY, isPlayer1);

        // Handle zoom
        HandleZoom(ref dist, isPlayer1);

        // Update camera position
        Vector3 direction = new Vector3(0, 0, -dist);
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 desiredPosition = targetPosition + rotation * direction;

        if (Physics.SphereCast(targetPosition, collisionRadius, desiredPosition - targetPosition,
            out RaycastHit hit, dist, collisionMask))
        {
            desiredPosition = targetPosition + (desiredPosition - targetPosition).normalized * (hit.distance - 0.1f);
        }

        cam.transform.position = desiredPosition;
        cam.transform.LookAt(targetPosition);
    }

    void HandleRotation(ref float rotX, ref float rotY, bool isPlayer1)
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minVerticalAngle, maxVerticalAngle);
    }

    void HandleZoom(ref float dist, bool isPlayer1)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        dist = Mathf.Clamp(dist - scroll * 5f, minDistance, maxDistance);
    }
}

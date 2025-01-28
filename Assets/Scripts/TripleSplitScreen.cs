using UnityEngine;

public class TripleSplitScreen : MonoBehaviour
{
    public Camera player1Camera;
    public Camera player2Camera;
    public Camera bubbleCamera;

    public Transform player1;
    public Transform player2;
    public Transform bubble;

    [Header("Third Person Settings")]
    public Vector3 cameraOffset = new Vector3(0, 5, -8);
    public float rotationSpeed = 180f;
    public float followSpeed = 5f;

    // Additional offset just for the bubble camera
    public Vector3 bubbleCameraOffset = new Vector3(0, 20, 0);

    void Start()
    {
        // Set up camera viewports
        player1Camera.rect = new Rect(0, 0, 0.33f, 1);
        bubbleCamera.rect = new Rect(0.33f, 0, 0.34f, 1);
        player2Camera.rect = new Rect(0.67f, 0, 0.33f, 1);

        // Configure cameras for third person view
        ConfigureCamera(player1Camera);
        ConfigureCamera(bubbleCamera);
        ConfigureCamera(player2Camera);
    }

    void ConfigureCamera(Camera cam)
    {
        // For players, a 30-degree tilt might be fine
        // We'll override the bubble camera’s tilt in LateUpdate or below
        cam.clearFlags = CameraClearFlags.Depth;
        cam.transform.rotation = Quaternion.Euler(30, 0, 0);
    }

    void LateUpdate()
    {
        UpdateThirdPersonCamera(player1Camera, player1);
        UpdateThirdPersonCamera(bubbleCamera, bubble, isBubble: true);
        UpdateThirdPersonCamera(player2Camera, player2);
    }

    // Overload to handle the bubble differently
    void UpdateThirdPersonCamera(Camera cam, Transform target, bool isBubble = false)
    {
        // Decide which offset to use
        Vector3 offsetToUse = isBubble ? bubbleCameraOffset : cameraOffset;

        // Calculate desired position
        Vector3 desiredPosition = target.position + target.TransformDirection(offsetToUse);

        // Smooth follow
        cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // If it's the bubble camera, aim it straight down, or at least more top-down
        if (isBubble)
        {
            // Look straight down at the bubble, or bubble + some height
            cam.transform.rotation = Quaternion.Euler(90f, 0, 0);
            // optional: tweak to center bubble in view
            cam.transform.LookAt(target.position);
        }
        else
        {
            // Normal third-person look
            cam.transform.LookAt(target.position + Vector3.up);
        }
    }
}

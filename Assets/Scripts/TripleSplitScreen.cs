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
        cam.clearFlags = CameraClearFlags.Depth;
        cam.transform.rotation = Quaternion.Euler(30, 0, 0);
    }

    void LateUpdate()
    {
        UpdateThirdPersonCamera(player1Camera, player1);
        UpdateThirdPersonCamera(bubbleCamera, bubble);
        UpdateThirdPersonCamera(player2Camera, player2);
    }

    void UpdateThirdPersonCamera(Camera cam, Transform target)
    {
        // Calculate desired position
        Vector3 desiredPosition = target.position + target.TransformDirection(cameraOffset);

        // Smooth follow
        cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // Look at target
        cam.transform.LookAt(target.position + Vector3.up);
    }
}

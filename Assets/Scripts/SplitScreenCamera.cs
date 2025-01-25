using UnityEngine;

public class SplitScreenCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -5);
    [SerializeField] private float smoothSpeed = 0.125f;

    private Camera player1Camera;
    private Camera player2Camera;

    private void Start()
    {
        SetupCameras();
    }

    private void LateUpdate()
    {
        if (player1 != null && player2 != null)
        {
            UpdateCameraPositions();
        }
    }

    private void SetupCameras()
    {
        if (player1Camera == null)
        {
            GameObject cam1 = new GameObject("Player1Camera");
            player1Camera = cam1.AddComponent<Camera>();
            cam1.transform.parent = transform;
        }

        if (player2Camera == null)
        {
            GameObject cam2 = new GameObject("Player2Camera");
            player2Camera = cam2.AddComponent<Camera>();
            cam2.transform.parent = transform;
        }

        player1Camera.rect = new Rect(0, 0, 0.5f, 1);
        player2Camera.rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    private void UpdateCameraPositions()
    {
        Vector3 desiredPosition1 = player1.position + cameraOffset;
        Vector3 smoothedPosition1 = Vector3.Lerp(player1Camera.transform.position, desiredPosition1, smoothSpeed);
        player1Camera.transform.position = smoothedPosition1;
        player1Camera.transform.LookAt(player1.position);

        Vector3 desiredPosition2 = player2.position + cameraOffset;
        Vector3 smoothedPosition2 = Vector3.Lerp(player2Camera.transform.position, desiredPosition2, smoothSpeed);
        player2Camera.transform.position = smoothedPosition2;
        player2Camera.transform.LookAt(player2.position);
    }
}

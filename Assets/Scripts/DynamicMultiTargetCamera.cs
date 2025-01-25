using UnityEngine;
using System.Collections.Generic;

public class DynamicMultiTargetCamera : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();

    [Header("Camera Settings")]
    [SerializeField] private float minZoom = 120f; // Increased 5x
    [SerializeField] private float maxZoom = 90f;  // Increased 5x
    [SerializeField] private float zoomLimiter = 150f; // Increased for wider view
    [SerializeField] private Vector3 offset = new Vector3(0, 175, -200); // Much higher and further back
    [SerializeField] private float minY = 100f; // Higher minimum height
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        newPosition.y = Mathf.Max(newPosition.y, minY);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        transform.LookAt(centerPoint);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform target in targets)
        {
            bounds.Encapsulate(target.position);
        }

        return Mathf.Max(bounds.size.x, bounds.size.z) * 5f; // Increased multiplier for much wider view
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1) return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform target in targets)
        {
            bounds.Encapsulate(target.position);
        }

        return bounds.center;
    }
}

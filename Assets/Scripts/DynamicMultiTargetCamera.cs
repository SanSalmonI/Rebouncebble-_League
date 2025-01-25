using UnityEngine;
using System.Collections.Generic;

public class DynamicMultiTargetCamera : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();

    [Header("Camera Settings")]
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 3f;
    [SerializeField] private float zoomLimiter = 15f;
    [SerializeField] private Vector3 offset = new Vector3(0, 35, -45);
    [SerializeField] private float minY = 15f;
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

        return Mathf.Max(bounds.size.x, bounds.size.z) * 1.5f;
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

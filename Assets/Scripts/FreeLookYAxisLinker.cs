using UnityEngine;
using Unity.Cinemachine;

public class ThirdPersonVerticalLinker : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vCam;
    public Transform bubble;

    [Header("Y Aim Settings")]
    public float minBubbleHeight = 0f;
    public float maxBubbleHeight = 10f;
    public float minPitch = -10f;  // Lower camera angle
    public float maxPitch = 30f;   // Higher camera angle
    public float smoothTime = 0.2f;

    private CinemachineThirdPersonFollow tpf;
    private float pitchVelocity;

    private void Start()
    {
        if (vCam != null)
            tpf = vCam.GetCinemachineComponent<CinemachineThirdPersonFollow>();
    }

    private void LateUpdate()
    {
        if (tpf == null || bubble == null) return;

        // 1) Get bubble's height (or bubble - player offset)
        float bh = bubble.position.y;

        // 2) Map that to 0..1
        float t = Mathf.InverseLerp(minBubbleHeight, maxBubbleHeight, bh);

        // 3) Convert that to a pitch angle
        float desiredPitch = Mathf.Lerp(minPitch, maxPitch, t);

        // 4) Smoothly move the camera pitch
        float currentPitch = tpf.CameraPitch; // The current vertical angle from ThirdPersonFollow
        float newPitch = Mathf.SmoothDamp(currentPitch, desiredPitch, ref pitchVelocity, smoothTime);

        // 5) Assign it back
        tpf.CameraPitch = newPitch;
    }
}

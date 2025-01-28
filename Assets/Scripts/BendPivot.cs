using UnityEngine;

public class BehindPivotUpdater : MonoBehaviour
{
    public Transform player;
    public Transform bubble;
    public float distanceBehind = 8f;
    public float heightOffset = 2f;

    void LateUpdate()
    {
        if (player == null || bubble == null) return;

        // Direction from bubble to player
        Vector3 direction = (player.position - bubble.position).normalized;
        // We want the pivot behind the player (along that direction), plus some height offset
        Vector3 behindPosition = player.position + direction * distanceBehind;
        behindPosition.y += heightOffset;

        transform.position = behindPosition;
        // Optionally match rotation if you want the pivot to face the bubble or the player's forward
        transform.LookAt(bubble.position);
    }
}

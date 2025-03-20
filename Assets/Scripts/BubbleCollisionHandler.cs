using UnityEngine;

[RequireComponent(typeof(BubbleMovement))]
public class BubbleCollisionHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private BubbleMovement bubbleMovement;

    private void Start()
    {
        bubbleMovement = GetComponent<BubbleMovement>();

        // Find the GameManager if not assigned
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameManager == null)
        {
            // Try to find the GameManager again if it wasn't found earlier
            gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager == null) return;
        }

        // Check if collision is with a player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerIdentifier playerIdentifier = collision.gameObject.GetComponent<PlayerIdentifier>();
            if (playerIdentifier != null)
            {
                gameManager.BubbleHitByPlayer(playerIdentifier.PlayerNumber);
            }
        }
        // Check if collision is with the ground
        else if (collision.gameObject.CompareTag("Ground"))
        {
            gameManager.BubbleHitGround();
        }
    }
}
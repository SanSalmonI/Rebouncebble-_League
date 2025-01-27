using UnityEngine;

public class Player1InputHandler : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Movement
        float verticalInput = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;

        playerController.inputDirection.z = verticalInput;
        playerController.rotationInput = horizontalInput;

        // Sprint
        playerController.isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Jump
        playerController.isJumping = Input.GetKey(KeyCode.Space);
    }
}

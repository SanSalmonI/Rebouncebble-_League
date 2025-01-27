using UnityEngine;

public class Player2InputHandler : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Movement
        float verticalInput = Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
        float horizontalInput = Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;

        playerController.inputDirection.z = verticalInput;
        playerController.rotationInput = horizontalInput;

        // Sprint
        playerController.isSprinting = Input.GetKey(KeyCode.RightShift);

        // Jump
        playerController.isJumping = Input.GetKey(KeyCode.RightAlt);
    }
}

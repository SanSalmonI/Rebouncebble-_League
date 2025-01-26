using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController playerController;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool sprintPressed;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        playerController.inputDirection.z = moveInput.y;
        playerController.rotationInput = moveInput.x;
    }

    public void OnJump(InputValue value)
    {
        playerController.isJumping = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        playerController.isSprinting = value.isPressed;
    }
}

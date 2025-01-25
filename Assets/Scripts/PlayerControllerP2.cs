using UnityEngine;

public class PlayerControllerP2 : PlayerController
{
    protected override void GetInput()
    {
        // Player 2 specific movement with arrow keys
        if (Input.GetKey(KeyCode.UpArrow)) inputDirection.z = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) inputDirection.z = -1;
        else inputDirection.z = 0;

        // Rotation with left/right arrows
        if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1;
        else rotationInput = 0;

        isJumping = Input.GetKey(KeyCode.RightControl);
        isSprinting = Input.GetKey(KeyCode.RightShift);
    }
}

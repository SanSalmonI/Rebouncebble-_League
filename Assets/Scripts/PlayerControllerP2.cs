using UnityEngine;

public class PlayerControllerP2 : PlayerController
{
    protected override void GetInput()
    {
        float vertical = 0;
        float horizontal = 0;

        if (Input.GetKey(KeyCode.Keypad8)) vertical += 1;
        if (Input.GetKey(KeyCode.Keypad5)) vertical -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) rotationInput = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) rotationInput = -1;
        if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) rotationInput = 0;

        inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        isJumping = Input.GetKeyDown(KeyCode.KeypadEnter);
        isSprinting = Input.GetKey(KeyCode.KeypadPlus);
    }

    protected override void HandleMovement()
    {
        float targetSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        Vector3 moveDirection = transform.forward * inputDirection.z;
        moveDirection.Normalize();

        if (moveDirection.magnitude > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        Vector3 targetVelocity = moveDirection * currentSpeed;
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, acceleration * Time.deltaTime);
        controller.Move(currentMoveVelocity * Time.deltaTime);
    }
}

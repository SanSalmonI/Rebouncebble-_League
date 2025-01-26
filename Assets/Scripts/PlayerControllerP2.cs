using UnityEngine;

public class PlayerControllerP2 : PlayerController
{
    protected override void HandleRotation()
    {
        float rotation = rotationInput * rotationSpeed * Time.deltaTime * 100f;
        transform.Rotate(Vector3.up * rotation);
    }
}

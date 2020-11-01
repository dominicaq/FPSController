using UnityEngine;

public class PlayerSwim : BaseController
{
    [Header("Swim")]
    public float ascendRate = 10;
    public float descendRate = 5;

    public override void Init()
    {
        base.Init();
    }

    public override void Tick()
    {
        ProcessMovement();
        Gravity();
    }

    public void Gravity()
    {
        if (!characterController.isGrounded)
            currentGravity -= descendRate * Time.deltaTime;
        else
            currentGravity = 0;

        currentGravity = Mathf.Clamp(currentGravity, -1, 3);
    }

    public void ProcessMovement()
    {
        transform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, 0);
        velocity = transform.rotation * inputVector * movementSpeed;
        velocity.y = currentGravity;

        characterController.Move(velocity * Time.deltaTime);
    }

    public override void Jump()
    {
        currentGravity += ascendRate;
    }
}
using UnityEngine;

public class PlayerLadder : BaseController
{
    [Header("Ladder Climbing")]
    public float climbSpeed = 2.0f;
    [Range(-90, 90)] public float climbAngle = 15.0f;
    private CameraController m_pCamera;

    public override void Init()
    {
        base.Init();
        m_pCamera = cameraTransform.GetComponent<CameraController>();
    }

    public override void Tick()
    {
        ProcessMovement();
    }

    public void ProcessMovement()
    {
        Vector3 ladderInput = new Vector3(inputVector.x, inputVector.z, inputVector.z);
        transform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, 0);
        
        velocity = transform.rotation * ladderInput * climbSpeed;

        if (!characterController.isGrounded)
            velocity.z = 0;

        if (m_pCamera.pitch >= climbAngle)
            velocity.y = -inputVector.z;
        
        velocity *= Mathf.Clamp(Mathf.Abs(m_pCamera.pitch), 0, 2);
        characterController.Move(velocity * Time.deltaTime);
    }
}
using System.Collections;
using UnityEngine;

public class PlayerController : BaseController
{
    #region Init
    private float m_InitMoveSpeed;
    private float m_InitPlayerHeight;
    private Vector3 m_InitCenter;
    private Vector3 m_CrouchCenter;

    #endregion
    
    #region Settings

    [Header("Gravity")]
    public float gravityModifier = -Physics.gravity.magnitude;
    public float maxFallSpeed = 50;
    public Vector3 hitNormal;

    [Header("Slope Sliding")]
    public bool isSliding = false;
    public float m_currentSlideSpeed;
    public float maximumSlopeAngle = 45.0f;

    [Header("Jump")]
    public bool isJumping = false;
    public float jumpHeight = 5f;
    public int maximumJumps = 1;
    private int m_JumpCounter;

    [Header("Crouch")]
    public bool isCrouching = false;
    public float crouchMovementSpeed = 2.8f;
    public float crouchHeight = 1.4f;
    public float crouchingTime = 15f;
    private bool crouchQueued = false;
    
    #endregion

    public override void Init()
    {
        base.Init();
        // Movement components
        m_InitMoveSpeed    = movementSpeed;

        // Crouching
        m_InitPlayerHeight = characterController.height;
        m_InitCenter       = characterController.center;
        m_CrouchCenter.y   = characterController.height * 0.125f;
    }

    public override void Tick()
    {
        ProcessMovement();
        Gravity();

        if(characterController.isGrounded) {
            m_JumpCounter = 0;
            isJumping = false;
        }

        if (hitNormal != Vector3.up && !isJumping)
            characterController.Move(Vector3.down * Time.deltaTime * 5);

        UpdateSlide();
        ProcessCrouchState();
    }

    private void ProcessMovement()
    {
        // Camera
        transform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, 0);

        velocity = (transform.rotation * inputVector) * movementSpeed;
        velocity.y = currentGravity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Gravity()
    {
        if(!characterController.isGrounded)
        {
            currentGravity += Time.deltaTime * gravityModifier;
            currentGravity = Mathf.Clamp(currentGravity, -maxFallSpeed, Mathf.Infinity);
        }
        else
            currentGravity = 0;
    }
    
    private void UpdateSlide()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 groundParallel = Vector3.Cross(Vector3.up, hit.normal);
            Vector3 slopeParallel = Vector3.Cross(groundParallel, hit.normal);

            // Current slope in degrees
            float currentSlope = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up));
            isSliding = characterController.isGrounded && currentSlope >= maximumSlopeAngle;

            if (isSliding)
            {
                m_currentSlideSpeed += Time.deltaTime * 0.5f;
                characterController.Move(slopeParallel.normalized * m_currentSlideSpeed);
            }
            else
                m_currentSlideSpeed = 0;
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        hitNormal = Vector3.zero;

        isJumping = false;
        isSliding = false;

        if(isCrouching)
            Crouch(false);
    }

    public override void Jump()
    {
        if(maximumJumps == 1 && !characterController.isGrounded)
            return;
        
        m_JumpCounter += 1;
        if (m_JumpCounter < maximumJumps + 1 && !isSliding)
        {
            currentGravity = jumpHeight;
            isJumping = true;
        }
    }

    public override void Crouch(bool toggleCrouch)
    {
        if (!isCrouching)
        {
            movementSpeed = crouchMovementSpeed;
            isCrouching = true;
        } 
        else if (!ObjectAbove(m_CrouchCenter.y + 0.75f) && toggleCrouch)
        {
            movementSpeed = m_InitMoveSpeed;
            isCrouching = false;
        }
        else if (!toggleCrouch && isCrouching)
            crouchQueued = true;
    }

    private void ProcessCrouchState()
    {
        Vector3 desiredCenter  = !isCrouching ? m_InitCenter : m_CrouchCenter;
        float desiredHeight    = !isCrouching ? m_InitPlayerHeight : crouchHeight;

        characterController.height = Mathf.Lerp(characterController.height, desiredHeight, crouchingTime * Time.deltaTime);
        characterController.center = Vector3.Lerp(characterController.center, desiredCenter, crouchingTime * Time.deltaTime);

        if (characterController.isGrounded)
        {
            if (!ObjectAbove(m_CrouchCenter.y + 0.75f))
                characterController.Move(Vector3.up * .1f);

            characterController.Move(Vector3.down * 10);
        }

        if(crouchQueued)
            UnCrouchWhenICan();
    }

    private void UnCrouchWhenICan()
    {
        if(!ObjectAbove(m_CrouchCenter.y + 0.75f) && isCrouching)
        {
            movementSpeed = m_InitMoveSpeed;
            crouchQueued = false;
            isCrouching = false;
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (ObjectAbove() && isJumping)
            currentGravity = -Mathf.Abs(currentGravity);

        hitNormal = hit.normal;
    }
}
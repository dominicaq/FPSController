using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Components
    public CharacterController characterController;
    private PlayerForce m_PlayerForce;
    
    #endregion

    #region Input

    public Vector3 velocity;
    
    [Header("Gravity")]
    public float worldGravity = Physics.gravity.magnitude;
    public float gravity;

    #endregion

    #region Init

    private Transform m_CameraTransform;
    private Vector3 m_InitCenter;
    private float m_InitMoveSpeed;
    private float m_InitPlayerHeight;
    private float m_InitCamHeight;
    private Vector3 m_CrouchCenter;
    private LayerMask m_HeadMask;

    #endregion
    
    #region Settings

    [Header("Movement")] 
    public bool enableInput = true;
    public float movementSpeed = 7.0f;
    public float crouchMovementSpeed = 2.8f;
    public bool enableCrouching = true;
    public bool enableCrouchToggle = false;
    
    [Header("Jump")]
    public bool isJumping = false;
    public float jumpHeight = 5f;
    public int maximumJumps = 1;
    private int m_JumpCounter;

    [Header("Crouch")]
    public bool isCrouching = false;
    public float crouchSpeed = 8f;
    public float crouchHeight = 1.4f;
    private float m_CrouchCamHeight;

    [Header("Slope Sliding")]
    public bool isSliding = false;
    private float m_currentSlideSpeed;
    
    #endregion
    
    private void Start()
    {
        // Movement components
        m_PlayerForce      = GetComponent<PlayerForce>();
        m_CameraTransform  = transform.GetChild(0);
        m_InitMoveSpeed    = movementSpeed;

        // Variables
        m_InitPlayerHeight = characterController.height;
        m_InitCenter       = characterController.center;

        m_HeadMask = ~LayerMask.GetMask("Ignore Player") + ~LayerMask.GetMask("Ignore Raycast");
        m_CrouchCamHeight  = m_InitPlayerHeight - crouchHeight;
        m_CrouchCenter.y   = characterController.height * 0.125f;
        m_InitCamHeight    = m_CameraTransform.localPosition.y;
    }

    public void PlayerInput()
    {
        if (Input.GetButtonDown("Jump"))
            Jump();

        if(!enableCrouching)
            return;

        if (Input.GetButtonDown("Crouch"))
            Crouch();
        else if (Input.GetButtonUp("Crouch") && isCrouching)
            Crouch();
    }
    
    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0, m_CameraTransform.eulerAngles.y, 0);
    }

    public void PlayerVelocity(float controlModifier)
    {
        gravity += Time.deltaTime * worldGravity;
        velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        if (velocity.sqrMagnitude > 1)
            velocity = velocity.normalized;
        
        velocity = (transform.rotation * velocity) * (movementSpeed * controlModifier);

        // Prevent player from excessive floating
        if (HeadCheck() && isJumping)
            gravity = -Mathf.Abs(gravity);
        
        velocity.y = gravity;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded)
        {
            gravity = 0;
            isJumping = false;
            m_JumpCounter = 0;
        }

        // Downward force on unevenground
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height * .75f))
        {
            if (hit.normal != Vector3.up && !isJumping)
                characterController.Move(Vector3.down * Time.deltaTime * 5);
        }
    }

    private void Jump()
    {
        m_JumpCounter += 1;

        if (m_JumpCounter < maximumJumps + 1)
        {
            if(!characterController.isGrounded && maximumJumps == 1 || isSliding)
                return;
            
            m_PlayerForce.AddYForce(jumpHeight);
        }
    }

    public void SlopeSliding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height, ~0, QueryTriggerInteraction.Ignore))
        {
            
            Vector3 temp = Vector3.Cross(hit.normal, hit.point - transform.position);
            Vector3 slopeDirection = Vector3.Cross(temp, hit.normal);

            isSliding = characterController.isGrounded && slopeDirection.y <= -0.45f;
            if (isSliding)
            {
                m_currentSlideSpeed += Time.deltaTime * 0.1f;
                characterController.Move(slopeDirection.normalized * m_currentSlideSpeed);
            }
            else
                m_currentSlideSpeed = 0;
        }
    }
    
    private void Crouch()
    {
        if (!isCrouching)
        {
            StartCoroutine(CrouchRoutine());
            movementSpeed = crouchMovementSpeed;

            isCrouching = true;
        }
        else if (!HeadCheck())
        {
            StartCoroutine(CrouchRoutine());
            movementSpeed = m_InitMoveSpeed;

            isCrouching = false;
        }
    }
    
    private IEnumerator CrouchRoutine()
    {
        // Setup variables depending on players current condition
        Vector3 desiredCenter  = isCrouching ? m_InitCenter : m_CrouchCenter;
        float desiredHeight    = isCrouching ? m_InitPlayerHeight : crouchHeight;
        float camDesiredHeight = isCrouching ? m_InitCamHeight : m_CrouchCamHeight;

        float crouchTime = 0f;
        while (crouchTime < 1f)
        {
            crouchTime += Time.deltaTime * crouchSpeed;

            characterController.height = Mathf.Lerp(characterController.height, desiredHeight, crouchTime);
            characterController.center = Vector3.Lerp(characterController.center, desiredCenter, crouchTime);

            Vector3 localCam = m_CameraTransform.localPosition;
            localCam.y = Mathf.Lerp(localCam.y, camDesiredHeight, crouchTime);
            m_CameraTransform.localPosition = localCam;

            // Ensures player stays where they crouched / uncrouched
            if (characterController.isGrounded)
            {
                if (!HeadCheck())
                    characterController.Move(Vector3.up * .1f);

                characterController.Move(Vector3.down * 10);
            }
            yield return null;
        }
    }

    public IEnumerator AdjustSpeed(float rate, float newSpeed)
    {
        float elapsedTime = 0.0f;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rate;
            movementSpeed = Mathf.Lerp(movementSpeed, newSpeed, elapsedTime);

            yield return null;
        }
    }
    
    public bool HeadCheck()
    {
        float rayLength = isCrouching ? m_CrouchCenter.y + 0.75f : .6f;

        return Physics.SphereCast(transform.position, characterController.radius, Vector3.up, out RaycastHit hitInfo, rayLength, m_HeadMask);
    }
}
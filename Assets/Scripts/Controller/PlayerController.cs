using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Components
    
    public CharacterController characterController;
    private PlayerMovementManager m_movementManager;
    
    #endregion

    #region Input

    private Vector3 m_playerVelocity;
    private float m_horizontal;
    private float m_vertical;
    
    [Header("Gravity")]
    public float worldGravity = Physics.gravity.magnitude;
    public float gravity;

    #endregion

    #region Init

    private Transform m_cameraTransform;
    private Vector3 m_initCenter;
    private float m_initMoveSpeed;
    private float m_initPlayerHeight;
    private float m_initCamHeight;
    private Vector3 m_crouchCenter;

    #endregion
    
    // Updated by other scripts
    [NonSerialized] public bool enableJumping = true;
    
    #region Settings

    [Header("Movement")] 
    public bool enableInput = true;
    public float movementSpeed = 7.0f;
    public float crouchMovementSpeed = 2.8f;
    public bool enableCrouching = true;
    public bool enableCrouchToggle = false;
    
    [Header("Jump")] 
    public float jumpHeight = 5f;
    public int maximumJumps = 1;
    private int m_jumpCounter;

    [Header("Crouch")] 
    public float crouchSpeed = 8f;
    public float crouchHeight = 1.4f;
    private float crouchCamHeight;

    [Header("Slope Sliding")]
    public float currentSlideSpeed;
    
    #endregion
    
    private void Start()
    {
        // Movement components
        m_movementManager = GetComponent<PlayerMovementManager>();
        m_cameraTransform = transform.GetChild(0);
        
        // Variables
        m_initPlayerHeight = characterController.height;
        m_initCenter = characterController.center;

        crouchCamHeight = m_initPlayerHeight - crouchHeight;
        m_crouchCenter.y = characterController.height * 0.125f;
        m_initCamHeight = m_cameraTransform.localPosition.y;
        m_initMoveSpeed = movementSpeed;
    }

    public void PlayerInput()
    {
        m_horizontal = Input.GetAxis("Horizontal");
        m_vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && enableJumping)
        {
            Jump();
        }

        if (Input.GetButtonDown("Crouch"))
        {
            Crouch();
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            Crouch();
        }
    }
    
    private void LateUpdate()
    {
        Vector3 bodyRot = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        transform.eulerAngles = bodyRot;
    }

    public void PlayerVelocity(float controlModifier)
    {
        gravity += Time.deltaTime * worldGravity;
        m_playerVelocity = new Vector3(m_horizontal, 0, m_vertical);
        
        if (m_playerVelocity.sqrMagnitude > 1)
            m_playerVelocity = m_playerVelocity.normalized;
        
        // Prevent player from excessive floating
        if (HeadCheck() && m_movementManager.isJumping)
            gravity = -Mathf.Abs(gravity);
        
        m_playerVelocity = (transform.rotation * m_playerVelocity) * (movementSpeed * controlModifier);
        m_playerVelocity.y = gravity;
        characterController.Move(m_playerVelocity * Time.deltaTime);

        if (characterController.isGrounded)
        {
            m_movementManager.isJumping = false;
            m_jumpCounter = 0;
            gravity = 0;
        }

        // Downward force on unevenground
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height * .75f))
        {
            if (hit.normal != Vector3.up && ! m_movementManager.isJumping)
            {
                float slopeStep = Time.deltaTime * 5;
                characterController.Move(Vector3.down * slopeStep);
            }
        }
    }

    private void Jump()
    {
        m_jumpCounter += 1;

        if (m_jumpCounter < maximumJumps + 1)
        {
            if(!characterController.isGrounded && maximumJumps == 1 || m_movementManager.isSliding)
                return;
            
            m_movementManager.playerForce.AddYForce(jumpHeight);
        }
    }

    public void SlopeSliding()
    {
        Vector3 slopeDirection = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 temp = Vector3.Cross(hit.normal, hit.point - transform.position);
            slopeDirection = Vector3.Cross(temp, hit.normal);
        }

        if (characterController.isGrounded && slopeDirection.y <= -0.45f)
        {
            m_movementManager.isSliding = true;
            currentSlideSpeed += Time.deltaTime * 0.1f;
            characterController.Move(slopeDirection.normalized * currentSlideSpeed);
        }
        else
        {
            m_movementManager.isSliding = false;
            currentSlideSpeed = 0;
        }
    }
    
    private void Crouch()
    {
        if (characterController.isGrounded)
        {
            if (!m_movementManager.isCrouching)
            {
                StartCoroutine(CrouchRoutine());
                movementSpeed = crouchMovementSpeed;

                m_movementManager.isCrouching = true;
            }
            else if (!HeadCheck())
            {
                StartCoroutine(CrouchRoutine());
                movementSpeed = m_initMoveSpeed;

                m_movementManager.isCrouching = false;
            }
        }
        else
        {
            StartCoroutine(CrouchOnLanding());
        }
    }

    private IEnumerator CrouchOnLanding()
    {
        while(!characterController.isGrounded)
            yield return null;
        
        Crouch();
    }
    
    private IEnumerator CrouchRoutine()
    {
        // Setup variables depending on players current condition
        Vector3 desiredCenter = m_movementManager.isCrouching ? m_initCenter : m_crouchCenter;
        float desiredHeight = m_movementManager.isCrouching ? m_initPlayerHeight : crouchHeight;
        float camDesiredHeight = m_movementManager.isCrouching ? m_initCamHeight : crouchCamHeight;

        float crouchTime = 0f;
        while (crouchTime < 1f)
        {
            crouchTime += Time.deltaTime * crouchSpeed;

            characterController.height = Mathf.Lerp(characterController.height, desiredHeight, crouchTime);
            characterController.center = Vector3.Lerp(characterController.center, desiredCenter, crouchTime);

            Vector3 localCam = m_cameraTransform.localPosition;
            localCam.y = Mathf.Lerp(localCam.y, camDesiredHeight, crouchTime);
            m_cameraTransform.localPosition = localCam;

            // Ensures player stays where they crouched / uncrouched
            if (characterController.isGrounded)
            {
                if (!HeadCheck())
                {
                    characterController.Move(Vector3.up * .1f);
                }

                characterController.Move(Vector3.down * 10);
            }
            yield return null;
        }
    }

    /// <summary> Coroutine, change move speed to a new speed at a desired rate </summary>
    /// <param name="rate"></param> <param name="newSpeed"></param>
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
    
    /// <summary> Returns true if an object is detected above players head </summary>
    public bool HeadCheck()
    {
        float rayLength = m_movementManager.isCrouching ? m_crouchCenter.y + 0.75f : .6f;

        return Physics.SphereCast(transform.position, characterController.radius, Vector3.up,
            out RaycastHit hitInfo, rayLength, ~LayerMask.GetMask("Ignore Player"));
    }
}
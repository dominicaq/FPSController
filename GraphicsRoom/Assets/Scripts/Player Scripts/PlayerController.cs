using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Input")]
    public float horizontal;
    public float vertical;

    [Header("Player Movement")]
    public float movementSpeed = 7.0f;
    public float crouchSpeed = 2.8f;
    public float jumpHeight = 2f;
    public float crouchRate = 10.0f;
    private float sParam = 0.0f;

    [Header("Base Stats")]
    private float baseMoveSpeed;
    private float baseJumpHeight;
    private float baseHeight;

    [Header("Gravity")]
    public float gravity = 6f;
    public float velocity = 0.0f;
    public float slopeRayLength = 1.5f;
    private PlayerForce forceModifier;

    [Header("Conditions")]
    public bool isCrouching = false;
    public bool isJumping = false;

    [Header("Character Controller")]
    private CharacterController playerCC;
    private Transform self;

    [Header("Character Camera")]
    private Transform playerCamera;
    private PlayerCamera cameraProperties;
    private CameraShake playerShake;
    
    void Awake()
    {
        // Player Camera
        playerCamera = transform.GetChild(0);
        cameraProperties = playerCamera.GetComponent<PlayerCamera>();
        playerShake = playerCamera.GetComponent<CameraShake>();

        // Character Controller and Player force modifier
        playerCC = GetComponent<CharacterController>();
        forceModifier = GetComponent<PlayerForce>();
        baseHeight = playerCC.height;

        // Base stats
        baseMoveSpeed = movementSpeed;
        baseJumpHeight = jumpHeight;
    }
    private void PlayerInput()
    {            
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Crouching
        if (Input.GetKeyDown(KeyCode.C) && !cameraProperties.cameraCanTransition)
            Crouch();    
    }

    void Update()
    {
        if (!cameraProperties.enableSpectator)
        {
            PlayerMove();
            PlayerInput();
            EssentialStatements();
        }
    }

    // For things that need constant updating
    private void EssentialStatements()
    {
        // Body rotation with camera
        Vector3 bodyRot = playerCamera.eulerAngles;
        bodyRot.x = 0f;
        transform.eulerAngles = bodyRot;
        
        // Prevent player from excessive floating
        if (HeadCheck() && isJumping)
        {
            velocity = -0.75f;
        }

        // Decelerate player when crouching
        if (isCrouching)
        {
            sParam += 2f * Time.deltaTime;
            movementSpeed = Mathf.Lerp(baseMoveSpeed, crouchSpeed, sParam);
        }
        else
            sParam = 0;
    }
    
    // Handles gravity, movement, and movement on terrain
    private void PlayerMove()
    {
        // Player Input
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Gravity
        velocity -= gravity * Time.deltaTime;

        Vector3 playerInput = new Vector3 (horizontal,0,vertical);

        // Prevent Execessive diagonal movement
        if(playerInput.sqrMagnitude > 1)
        {   
            playerInput = playerInput.normalized;
        }

        // Finalization
        playerInput = transform.rotation * playerInput * movementSpeed;
        playerInput.y = velocity * gravity;

        playerCC.Move(playerInput * Time.deltaTime);
        
        if((HeadCheck() && isJumping) || playerCC.isGrounded)
        {
            if(velocity < -7f)
            {
                playerShake.InduceStress(Mathf.Abs(velocity));
            }

            isJumping = false;
            velocity = 0;
        }

        // Slope Handling
        if(OnSlope())
        {
            playerCC.Move(Vector3.down * 5 * Time.deltaTime);
        }
    }

    private void Jump()
    {
        if (playerCC.isGrounded)
        {
            isJumping = true;
            forceModifier.AddUpwardForce(jumpHeight);
        }
    }

    private void Crouch()
    {
        if (!isCrouching)
        {
            playerCC.height = baseHeight * .6f;

            isCrouching = true;
            cameraProperties.cameraCanTransition = true;

            jumpHeight = jumpHeight * .3f;
        }
        else if (!HeadCheck())
        {
            playerCC.height = baseHeight;

            isCrouching = false;
            cameraProperties.cameraCanTransition = true;

            // Movement values
            movementSpeed = baseMoveSpeed;
            jumpHeight = baseJumpHeight;
        }

        // Adjust player position for resize
        if (playerCC.isGrounded)
        {
            if (!HeadCheck())
            {
                playerCC.Move(Vector3.up * .8f);
            }
            playerCC.Move(Vector3.down * 10);
        }
    }

    // True if object is decteted above head, false if not
    private bool HeadCheck()
    {
        float rayLength = 1.0f;

        if(isJumping)
            rayLength = .6f;

        return Physics.SphereCast(transform.position, playerCC.radius, Vector3.up, out RaycastHit hitInfo, rayLength);
    }

    // Check if player is on a slope
    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeRayLength))
        {
            if (hit.normal != Vector3.up)
                return true;
        }

        return false;
    }
}

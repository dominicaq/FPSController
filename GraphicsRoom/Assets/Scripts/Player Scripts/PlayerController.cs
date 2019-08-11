using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Input")]
    public bool RestrictAirControl = false;
    private float horizontal;
    private float vertical;
    private Vector3 moveDirection;
    private bool enableJumping = true;
    [SerializeField] private bool disableCrouchToggle = false;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float crouchSpeed = 2.8f;
    public float crouchRate = 10.0f;
    private float sParam = 0.0f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private bool enableMultiJump = true;
    [SerializeField] private int maximumJumps = 2;
    private int jumpCounter = 0;
    private bool canJumpInAir = false;

    [Header("Base Stats")]
    private float baseMoveSpeed;
    private float baseJumpHeight;
    private float baseHeight;

    [Header("Gravity")]
    public float velocity = 0.0f;
    [SerializeField] private float gravity = 6f;
    [SerializeField] private float slopeRayLength = 1.5f;
    private PlayerForce forceModifier;

    [Header("Player Conditions")]
    public bool isCrouching = false;
    public bool isJumping = false;
    public bool isFlying = false;
    public bool isClimbingLadder = false;

    [Header("Character Controller")]
    private CharacterController playerCC;
    private Transform self;

    [Header("Character Camera")]
    private Transform playerCamera;
    private PlayerCamera cameraProperties;
    private CameraShake playerShake;
    
    // Gets componenets and sets base stats
    private void Awake()
    {
        // Player Camera
        playerCamera = transform.GetChild(0);
        cameraProperties = playerCamera.GetComponent<PlayerCamera>();
        playerShake = playerCamera.GetComponent<CameraShake>();

        // Character Controller and Player force modifier
        playerCC = GetComponent<CharacterController>();
        forceModifier = GetComponent<PlayerForce>();
        
        // Base Variables
        baseMoveSpeed = movementSpeed;
        baseHeight = playerCC.height;
        baseJumpHeight = jumpHeight;
    }
    private void PlayerInput()
    {
        // Player Input
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && enableJumping)
        {
            Jump();
        }

        // Crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.C) && isCrouching && disableCrouchToggle)
        {
            Crouch();
        }
    }

    private void Update()
    {
        if(isClimbingLadder)
        {
            LadderMovement();
        }
        else
        {
            enableJumping = isCrouching ? false : true;
            PlayerMove();
        }

        RotateBodyWithCamera();
        PlayerInput();
        PlayerMisc();
    }

    // For things that need constant updating
    private void PlayerMisc()
    {        
        // Prevent player from excessive floating when jumping
        if (HeadCheck() && isJumping && !isFlying)
        {
            velocity = -0.5f;
        }

        // Decelerate player movement when crouching
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
        // Gravity
        velocity -= gravity * Time.deltaTime;

        // Air Control
        if(!playerCC.isGrounded && RestrictAirControl)
        {
            horizontal = horizontal / 2;
            vertical = vertical / 4;
        }

        moveDirection = new Vector3 (horizontal,0,vertical);

        // Prevent Execessive diagonal movement
        if(moveDirection.sqrMagnitude > 1)
        {   
            moveDirection = moveDirection.normalized;
        }

        // Finalization
        moveDirection = transform.rotation * moveDirection * movementSpeed;
        moveDirection.y = velocity * gravity;
        
        playerCC.Move(moveDirection * Time.deltaTime);
        
        // Grounding
        if(playerCC.isGrounded)
        {
            if(velocity < -7f)
            {
                playerShake.InduceStress(Mathf.Abs(velocity));
            }

            RestrictAirControl = false;
            isJumping = false;
            canJumpInAir = true;
            jumpCounter = 0;
            velocity = 0;
        }

        // Slope Handling
        if(OnSlope())
        {
            playerCC.Move(Vector3.down * 5 * Time.deltaTime);
        }
    }

    private void LadderMovement()
    {
        velocity = 0;
        enableJumping  = false;

        moveDirection = new Vector3(horizontal, vertical, 0);

        if(moveDirection.sqrMagnitude > 1)
        {   
            moveDirection = moveDirection.normalized;
        }

        moveDirection = transform.rotation * moveDirection * movementSpeed;
        playerCC.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        // Remove a jump if player is in air
        if(!playerCC.isGrounded && !isJumping && canJumpInAir)
        {   
            jumpCounter += 1;
            canJumpInAir = false;
        }

        if(isJumping)
        {
            RestrictAirControl = false;
            
            // Multi jump
            if(maximumJumps != jumpCounter)
                jumpCounter += 1;
        }

        // AddForce() will make isJumping true
        if (playerCC.isGrounded || (jumpCounter != maximumJumps && enableMultiJump))
        {
            forceModifier.AddForce(jumpHeight);
        }
    }

    // TODO: REDO CROUCHING
    private void Crouch()
    {
        if (!isCrouching)
        {
            playerCC.height = baseHeight * .6f;

            isCrouching = true;
            cameraProperties.cameraCanTransition = true;
        }
        else if (!HeadCheck())
        {
            playerCC.height = baseHeight;

            movementSpeed = baseMoveSpeed;
            isCrouching = false;
            cameraProperties.cameraCanTransition = true;
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
    public bool HeadCheck()
    {
        float rayLength = .6f;
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

    // Body rotation with camera
    private void RotateBodyWithCamera()
    {
        Vector3 bodyRot = playerCamera.eulerAngles;
        bodyRot.x = 0f;
        transform.eulerAngles = bodyRot;
    }

    private void OnDrawGizmos()
    { 
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * .6f, .5f);
    }
}

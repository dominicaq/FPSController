using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Input")]
    private float horizontal;
    private float vertical;
    [SerializeField] private bool disableCrouchToggle = false;
    public bool RestrictAirControl = false;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float crouchSpeed = 2.8f;
    public float crouchRate = 10.0f;
    private float sParam = 0.0f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2f;
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
    
    private void Awake()
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
        {
            Jump();
        }

        // Crouching
        if(disableCrouchToggle)
        {
            if(Input.GetKeyDown(KeyCode.C) && !cameraProperties.cameraCanTransition)
            {
                Crouch();
            }

            if(Input.GetKeyUp(KeyCode.C) && !cameraProperties.cameraCanTransition && isCrouching)
            {
                Crouch();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C) && !cameraProperties.cameraCanTransition)
            {
                Crouch();
            }
        }
    }

    private void Update()
    {
        PlayerMove();
        PlayerInput();
        PlayerMisc();
    }

    // For things that need constant updating
    private void PlayerMisc()
    {        
        // Prevent player from excessive floating
        if (HeadCheck() && isJumping && !isFlying)
        {
            velocity = -0.5f;
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
        // Body rotation with camera
        Vector3 bodyRot = playerCamera.eulerAngles;
        bodyRot.x = 0f;
        transform.eulerAngles = bodyRot;

        // Player Input
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Air Control
        if(!playerCC.isGrounded && RestrictAirControl)
        {
            horizontal = horizontal / 2;
            vertical = vertical / 4;
        }

        // Gravity
        velocity -= gravity * Time.deltaTime;

        // Ladder handling
        Vector3 playerInput; 
        if (isClimbingLadder)
        {
            playerInput = new Vector3(horizontal, vertical, 0);
        }
        else
        {
            playerInput = new Vector3 (horizontal,0,vertical);
        }

        // Prevent Execessive diagonal movement
        if(playerInput.sqrMagnitude > 1)
        {   
            playerInput = playerInput.normalized;
        }

        // Finalization
        playerInput = transform.rotation * playerInput * movementSpeed;

        if(!isClimbingLadder)
        {
            playerInput.y = velocity * gravity;
        }
        else
        {
            velocity = 0;
        }
        

        playerCC.Move(playerInput * Time.deltaTime);
        
        if(playerCC.isGrounded)
        {
            if(velocity < -7f)
            {
                playerShake.InduceStress(Mathf.Abs(velocity));
            }

            isJumping = false;
            RestrictAirControl = false;
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
            
            if(maximumJumps != jumpCounter)
                jumpCounter += 1;
        }

        // AddForce() will make isJumping true
        if (playerCC.isGrounded || (jumpCounter != maximumJumps && enableMultiJump))
        {
            forceModifier.AddForce(jumpHeight*2);
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

    private void OnDrawGizmos()
    { 
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * .6f, .5f);
    }
}

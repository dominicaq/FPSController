using System.Collections;
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
    [SerializeField] private float crouchMovementSpeed = 2.8f;

    [Header("Crouching")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private Vector3 crouchCenter;
    [SerializeField] private float crouchDecelerateSpeed = 2.0f;
    [SerializeField] private float crouchTransitionDuration = 1f;
    [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
    private bool duringCrouchAnimation;
    private float crouchCamHeight;
    private float initCamHeight;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private bool enableMultiJump = true;
    [SerializeField] private int maximumJumps = 2;
    private int jumpCounter = 0;
    private bool canJumpInAir = false;

    [Header("Init Variables")]
    private Vector3 initCenter;
    private float initMoveSpeed;
    private float initJumpHeight;
    private float initPlayerHeight;

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
    private void Start()
    {
        // Player Camera
        playerCamera = transform.GetChild(0);
        cameraProperties = playerCamera.GetComponent<PlayerCamera>();
        playerShake = playerCamera.GetComponent<CameraShake>();

        // Character Controller and Player force modifier
        playerCC = GetComponent<CharacterController>();
        forceModifier = GetComponent<PlayerForce>();
        
        // Init Variables
        initPlayerHeight = playerCC.height;
        initCenter = playerCC.center;

        initCamHeight = playerCamera.localPosition.y;
        crouchCenter = (crouchHeight / 2f + playerCC.skinWidth) * Vector3.up;
        crouchCamHeight = initPlayerHeight - crouchHeight;

        initMoveSpeed = movementSpeed;
        initJumpHeight = jumpHeight;
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
        if (Input.GetKeyDown(KeyCode.C) && !duringCrouchAnimation)
        {
            Crouch();
        }
        else if (!Input.GetKey(KeyCode.C) && isCrouching && !duringCrouchAnimation && disableCrouchToggle)
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

        // Ensure movespeed is always initspeed when not crouching
        if(!isCrouching)
        {
            movementSpeed = initMoveSpeed;
        }
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

    private void Crouch()
    {
        if(!isCrouching)
        {
            StartCoroutine("CrouchRoutine");
            StartCoroutine("CrouchDecelerateRoutine");
            isCrouching = true;
        }
        else if (!HeadCheck())
        {
            StartCoroutine("CrouchRoutine");
            movementSpeed = initMoveSpeed;
            isCrouching = false;
        }
    }

    private IEnumerator CrouchRoutine()
    {
        duringCrouchAnimation = true;

        float crouchParam = 0f;
        float smoothCrouchParam = 0f;

        float crouchSpeed = 1f / crouchTransitionDuration;
        float currentHeight = playerCC.height;
        Vector3 currentCenter = playerCC.center;
        
        float desiredHeight = isCrouching ? initPlayerHeight : crouchHeight;
        Vector3 desiredCenter = isCrouching ? initCenter : crouchCenter;

        Vector3 camPos = playerCamera.localPosition;
        float camCurrentHeight = camPos.y;
        float camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

        while(crouchParam < 1f)
        {
            crouchParam += Time.deltaTime * crouchSpeed;
            smoothCrouchParam = crouchTransitionCurve.Evaluate(crouchParam);

            playerCC.height = Mathf.Lerp(currentHeight, desiredHeight, smoothCrouchParam);
            playerCC.center = Vector3.Lerp(currentCenter, desiredCenter, smoothCrouchParam);

            camPos.y = Mathf.Lerp(camCurrentHeight,camDesiredHeight, smoothCrouchParam);
            playerCamera.localPosition = camPos;

            // Adjust player position for resize
            if (playerCC.isGrounded)
            {
                if (!HeadCheck())
                {
                    playerCC.Move(Vector3.up * .8f);
                }
                playerCC.Move(Vector3.down * 10);
            }

            yield return null;
        }

        duringCrouchAnimation = false;
    }

    private IEnumerator CrouchDecelerateRoutine()
    {
        // Decelerate player movement when crouching
        float elapsedTime = 0.0f;

        while(elapsedTime < 1f)
        {
            elapsedTime += crouchDecelerateSpeed * Time.deltaTime;
            movementSpeed = Mathf.Lerp(initMoveSpeed, crouchMovementSpeed, elapsedTime);

            yield return null;
        }
    }

    // True if object is decteted above head, false if not
    public bool HeadCheck()
    {
        float rayLength = isCrouching ? initPlayerHeight : .6f;
        return Physics.SphereCast(transform.position, playerCC.radius, Vector3.up, out RaycastHit hitInfo, initPlayerHeight);
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
        float rayLength = isCrouching ? initPlayerHeight : .6f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * rayLength, .5f);
    }
}

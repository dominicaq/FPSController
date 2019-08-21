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
    [SerializeField] private bool enableCrouchToggle = false;
    private bool enableCrouching = false;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float crouchMovementSpeed = 2.8f;

    [Header("Crouching")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.25f, 0);
    [SerializeField] private float crouchDecelerateSpeed = 2.0f;
    [SerializeField] private float crouchTransitionDuration = 1f;
    [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
    private bool duringCrouchAnimation;
    private bool crouchOnLanding;
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
    [SerializeField] private bool enableIsGrounded = true;
    private PlayerForce forceModifier;

    [Header("Slope Sliding")]
    [SerializeField] private float maximumSlidingVelocity = -6f;
    private Vector3 incomingVec;
    private Vector3 slopeDirection;

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
    private CameraShake playerShake;
    private PlayerCamera cameraProperties;


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

        crouchCamHeight = initPlayerHeight - crouchHeight;
        initCamHeight = playerCamera.localPosition.y;
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
        if (Input.GetButtonDown("Crouch") && !duringCrouchAnimation)
        {
            Crouch();
            crouchOnLanding = !playerCC.isGrounded ? true : false;
        }
        else if (!Input.GetKey(KeyCode.C) && isCrouching && !duringCrouchAnimation && !enableCrouchToggle)
        {
            Crouch();
            crouchOnLanding = !playerCC.isGrounded ? true : false;
        }

        if(Input.GetButtonUp("Crouch"))
        {
            crouchOnLanding = false;
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
        SlideOffSlope();
        PlayerInput();
        PlayerMisc();
    }

    // For things that need constant updating
    private void PlayerMisc()
    {        
        // Prevent player from excessive floating when jumping
        if (HeadCheck() && isJumping && !isFlying && !isCrouching)
        {
            velocity = -0.5f;
        }

        // Perform crouch upon landing
        if(playerCC.isGrounded && crouchOnLanding)
        {
            Crouch();
            crouchOnLanding = false;
        }

        // Ensure movespeed is always initSpeed when not crouching
        if(!isCrouching)
        {
            movementSpeed = initMoveSpeed;
        }
    }
    
    // Handles gravity, movement, and movement on terrain
    private void PlayerMove()
    {
        // Gravity
        velocity -= Time.deltaTime * gravity;

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
        if(playerCC.isGrounded && enableIsGrounded)
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
        if(playerCC.isGrounded && enableCrouching)
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
                    playerCC.Move(Vector3.up * .1f);
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
            if(!isFlying && !isJumping)
            {
                elapsedTime += crouchDecelerateSpeed * Time.deltaTime;
                movementSpeed = Mathf.Lerp(initMoveSpeed, crouchMovementSpeed, elapsedTime);
            }

            yield return null;
        }
    }

    private void LadderMovement()
    {
        velocity = 0;
        enableJumping  = false;

        moveDirection = Vector3.up * vertical;

        if(moveDirection.sqrMagnitude > 1)
        {   
            moveDirection = moveDirection.normalized;
        }

        moveDirection = transform.rotation * moveDirection * movementSpeed;
        playerCC.Move(moveDirection * Time.deltaTime);
    }
    
    // True if object is decteted above head, false if not
    public bool HeadCheck()
    {
        float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
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

    private void SlideOffSlope()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, playerCC.radius, Vector3.down, out hit, 1) && !isCrouching)
        {
            // Standing ray
            incomingVec = hit.point - transform.position;
            slopeDirection =  Vector3.Reflect(incomingVec, hit.normal);
        }
        else if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            // Crouch ray
            incomingVec = hit.point - transform.position;
            slopeDirection =  Vector3.Reflect(incomingVec, hit.normal);
        }

        if(playerCC.isGrounded)
        {
            if(slopeDirection.y <= 0.1f && !isFlying)
            {
                if(velocity < maximumSlidingVelocity)
                    velocity = maximumSlidingVelocity;

                Vector3 slideVector = Vector3.down + slopeDirection * Time.deltaTime * Mathf.Abs(velocity);
                playerCC.Move(slideVector);
                
                RestrictAirControl = true;
                enableIsGrounded = false;
                enableCrouching = false;
                enableJumping = false;
            }
            else
            {
                RestrictAirControl = false;
                enableIsGrounded = true;
                enableCrouching = true;

                if(!isCrouching)
                    enableJumping = true;
            }
        }
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
        float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * rayLength, .5f);
    }
}

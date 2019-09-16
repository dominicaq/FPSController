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
    private bool enableCrouching = true;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float crouchMovementSpeed = 2.8f;
    [SerializeField] private float ladderAngle = 5.0f;

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
    [SerializeField] private float slopeRayLength = 1.5f;
    private PlayerForce forceModifier;

    [Header("Slope Sliding")]
    [SerializeField] private float slipeSpeed = 0.0f;
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
    private PlayerCamera playerCameraScript;


    // Gets componenets and sets base stats
    private void Start()
    {
        // Player Camera
        playerCamera = transform.GetChild(0);
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
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
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && enableJumping)
        {
            Jump();
        }

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
        PlayerInput();
        PlayerMisc();
    }

    private void LateUpdate() 
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
        SlideOffSteepSlope();
    }

    // For things that need constant updating
    private void PlayerMisc()
    {        
        // Prevent player from excessive floating when jumping
        if (HeadCheck() && isJumping && !isFlying)
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
    
    // Gravity and movement vector
    private void PlayerMove()
    {
        // Gravity
        velocity += Time.deltaTime * Physics.gravity.y;

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
        moveDirection.y = velocity;
        playerCC.Move(moveDirection * Time.deltaTime);
        
        if(playerCC.isGrounded)
        {
            if(velocity < -7f)
            {
                playerShake.InduceStress(Mathf.Abs(velocity));
            }

            canJumpInAir = true;
            isJumping = false;
            jumpCounter = 0;
            
            velocity = 0;
        }

        if(IsOnSlope())
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
                StartCoroutine("MovespeedDecelerateRoutine");
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

    private IEnumerator MovespeedDecelerateRoutine()
    {
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

    // TODO: Rework ladder movement
    private void LadderMovement()
    {
        velocity = 0;
        enableJumping  = false;

        Vector3 ladderVec = new Vector3 (horizontal,vertical,0);
        if(ladderVec.sqrMagnitude > 1)
            moveDirection = moveDirection.normalized;
        
        // Move controller based on camera rotation
        if(playerCameraScript.isLookingUp(ladderAngle))
        {
            ladderVec.y = vertical;
        }
        else if (playerCameraScript.isLookingDown(ladderAngle))
        {
            ladderVec.y = -vertical;
        }

        float moveRate = (playerCameraScript.getPitch() * 0.1f);
        ladderVec = ladderVec * moveRate;
        
        playerCC.Move(transform.rotation * ladderVec * Time.deltaTime);
    }
    
    // True if object is decteted above head, false if not
    public bool HeadCheck()
    {
        float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
        return Physics.SphereCast(transform.position, playerCC.radius, Vector3.up, out RaycastHit hitInfo, rayLength);
    }

    private bool IsOnSlope()
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

    private void SlideOffSteepSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            incomingVec = hit.point - transform.position;
            slopeDirection =  Vector3.Reflect(incomingVec, hit.normal);
        }

        if(slopeDirection.y <= -0.1f && !isFlying && !isJumping)
        {
            slipeSpeed += Time.deltaTime * 1.5f;

            playerCC.Move(Vector3.down * 4 * Time.deltaTime);
            playerCC.Move(slopeDirection * slipeSpeed * Time.deltaTime);

            enableJumping = false;
        }
        else if (playerCC.isGrounded || isFlying)
        {
            slipeSpeed = 0;       
            enableJumping = true;
        }
    }

    private void RotateBodyWithCamera()
    {
        Vector3 bodyRot = playerCamera.eulerAngles;
        bodyRot.x = 0f;
        transform.eulerAngles = bodyRot;
    }

    private void OnTriggerStay(Collider other)
    {
        isClimbingLadder = other.transform.tag == "Ladder";
    }

    private void OnTriggerExit(Collider other)
    {
        if(isClimbingLadder)
            isClimbingLadder = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * rayLength, .5f);
    }
}

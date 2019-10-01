using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Input")]
    public Vector3 velocity;
    [SerializeField] private bool enableCrouchToggle = false;
    private float horizontal;
    private float vertical;
    private bool enableJumping = true;
    private bool enableCrouching = true;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float crouchMovementSpeed = 2.8f;
    [SerializeField] private float ladderAngle = 5.0f;

    [Header("Crouching")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.25f, 0);
    [SerializeField] private float crouchDecelerateRate = 2.0f;
    [SerializeField] private float crouchTransitionDuration = 1f;
    [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
    private bool isCurrentlyCrouching;
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
    public float gravity = 0;
    [SerializeField] private float worldGravity = Physics.gravity.y;
    private PlayerForce forceModifier;

    [Header("Slope Sliding")]
    [SerializeField] private float slipeSpeed = 0.0f;
    [SerializeField] private float slideRate = 1.5f;
    private Vector3 incomingVec;
    private Vector3 slopeDirection;

    [Header("Player Conditions")]
    public bool isCrouching = false;
    public bool isJumping = false;
    public bool isFlying = false;
    public bool isClimbingLadder = false;
    public bool isSwimming = false;

    [Header("Character Controller")]
    private CharacterController playerCC;

    [Header("Character Camera")]
    private Transform playerCamera;
    private PlayerCamera playerCameraScript;


    // Gets componenets and sets base stats
    private void Start()
    {
        // Player Camera
        playerCamera = transform.GetChild(0);
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();

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

        if (Input.GetButtonDown("Jump") && enableJumping && !isSwimming)
        {
            Jump();
        }

        if (Input.GetButtonDown("Crouch") && !isCurrentlyCrouching)
        {
            Crouch();
            crouchOnLanding = !playerCC.isGrounded ? true : false;
        }
        else if (!Input.GetButton("Crouch") && isCrouching && !isCurrentlyCrouching && !enableCrouchToggle)
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
            LadderVelocity();
        }
        else if (isSwimming)
        {
            SwimVelocity();
        }
        else
        {
            enableJumping = isCrouching ? false : true;
            PlayerVelocity();
            SlideOffSlope();
        }

        RotateBodyWithView();
    }

    // For things that need constant updating
    private void PlayerMisc()
    {
        // Prevent player from excessive floating when jumping
        if (HeadCheck() && isJumping && !isFlying)
        {
            gravity = -0.5f;
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
    
    // Main movement function
    private void PlayerVelocity()
    {
        // Gravity
        gravity += Time.deltaTime * worldGravity;
        velocity = new Vector3 (horizontal,0,vertical);

        // Prevent Execessive diagonal movement
        if(velocity.sqrMagnitude > 1)
            velocity = velocity.normalized;

        // Finalization
        velocity = transform.rotation * velocity * movementSpeed;
        velocity.y = gravity;
        playerCC.Move(velocity * Time.deltaTime);
        
        if(playerCC.isGrounded)
        {
            canJumpInAir = true;
            isJumping = false;
            jumpCounter = 0;
            gravity = 0;
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

        // Multi jump
        if(isJumping && maximumJumps != jumpCounter)
        {   
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
                StartCoroutine(CrouchRoutine());

                if(!isFlying && !isJumping)
                    StartCoroutine(AdjustSpeedRoutine(crouchDecelerateRate, crouchMovementSpeed));

                isCrouching = true;
            }
            else if (!HeadCheck())
            {
                StartCoroutine(CrouchRoutine());
                movementSpeed = initMoveSpeed;
                isCrouching = false;
            }
        }
    }

    private IEnumerator CrouchRoutine()
    {
        isCurrentlyCrouching = true;

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

        isCurrentlyCrouching = false;
    }

    public IEnumerator AdjustSpeedRoutine(float rate, float newSpeed)
    {
        float elapsedTime = 0.0f;

        while(elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rate;
            movementSpeed = Mathf.Lerp(movementSpeed, newSpeed, elapsedTime);

            yield return null;
        }
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            if (hit.normal != Vector3.up)
                return true;
        }

        return false;
    }

    private void SlideOffSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            incomingVec = hit.point - transform.position;
            slopeDirection =  Vector3.Reflect(incomingVec, hit.normal);
        }

        if(slopeDirection.y <= -0.1f && !isFlying && !isJumping)
        {
            slipeSpeed += Time.deltaTime * slideRate;

            playerCC.Move(Vector3.down * 5 * Time.deltaTime);
            playerCC.Move(slopeDirection * slipeSpeed * Time.deltaTime);

            enableJumping = false;
        }
        else if (playerCC.isGrounded || isFlying)
        {
            slipeSpeed = 0;       
            enableJumping = true;
        }
    }

    private void RotateBodyWithView()
    {
        Vector3 bodyRot = playerCamera.eulerAngles;
        bodyRot.x = 0f;
        transform.eulerAngles = bodyRot;
    }

    private void LadderVelocity()
    {
        gravity = 0;
        enableJumping  = false;

        if(playerCC.isGrounded)
        {
            velocity = new Vector3 (horizontal,vertical,vertical);
        }
        else
        {
            velocity = new Vector3 (horizontal,vertical,0);
        }
        
        if (playerCameraScript.isLookingDown(ladderAngle))
            velocity.y = -vertical;

        float moveRate = (playerCameraScript.getPitch() * 0.1f);
        velocity = velocity * moveRate;
        
        playerCC.Move(transform.rotation * velocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if((playerCC.isGrounded || isJumping || isFlying) && !isSwimming && other.transform.tag == "Ladder")
            isClimbingLadder = true;

        if(other.transform.tag =="Water")
            isSwimming = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(isClimbingLadder)
            isClimbingLadder = false;
        
        if(isSwimming)
            isSwimming = false;
    }

    // Early version of swimming
    private void SwimVelocity()
    {
        enableJumping = false;
        isJumping = false;
        isFlying = false;
        
        velocity = new Vector3 (horizontal,0,vertical);

        // Prevent Execessive diagonal movement
        if(velocity.sqrMagnitude > 1)
            velocity = velocity.normalized;

        if (Input.GetButton("Jump"))
        {
            gravity += 10f * Time.deltaTime;

            if (gravity >= 3)
                gravity = 3;
        }
        
        if (gravity < -1 && !playerCC.isGrounded)
        {
            gravity += -worldGravity * Time.deltaTime;
        }
        else
        {
            gravity -= 5f * Time.deltaTime;

            if (gravity <= -1)
                gravity = -1;
        }

        // Finalization
        velocity = transform.rotation * velocity * movementSpeed;
        velocity.y = gravity;
        playerCC.Move(velocity * Time.deltaTime);
    }

    // Debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * rayLength, .5f);
    }
}

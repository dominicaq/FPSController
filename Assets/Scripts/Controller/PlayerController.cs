using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")] 
        [NonSerialized] public Vector3 inputVelocity;
        [NonSerialized] public float horizontal;
        [NonSerialized] public float vertical;

        [Header("Movement Settings")] 
        public float movementSpeed = 7.0f;
        [SerializeField] private float crouchMovementSpeed = 2.8f;

        [Header("Gravity")] 
        [SerializeField] private float worldGravity = Physics.gravity.y;
        public float gravity = 0;

        [Header("Components")] 
        [NonSerialized] public CharacterController characterController;
        [NonSerialized] public PlayerCamera cameraProperties;
        private PlayerLadder playerLadder;
        private PlayerForce forceModifier;
        private PlayerSwim playerSwim;
        private Transform playerCamera;

        [Header("Jump Settings")] 
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private int maximumJumps = 2;
        private int jumpCounter;
        private bool canJumpInAir;
        
        [Header("Crouch Settings")] 
        [SerializeField] private float crouchHeight = 1.4f;
        [SerializeField] private float crouchTransitionDuration = 0.25f;
        private float crouchDecelerateRate = 2.0f;
        private bool crouchOnLanding;
        private float crouchCamHeight;

        [Header("Slope Sliding")]
        [SerializeField] private float slideRate = 1.5f;
        private float currentSlideSpeed = 0.0f;
        private float timedDelay;
        private Vector3 slopeDirection;
        private float slideDelay = 0.5f;

        [Header("Init Variables")] 
        private Vector3 initCenter;
        private float initMoveSpeed;
        private float initPlayerHeight;
        private float initCamHeight;
        private Vector3 crouchCenter;
        
        [Header("Player Conditions")] 
        [NonSerialized] 
        public bool isClimbingLadder = false,
            isCrouching = false,
            isJumping = false,
            isFlying = false,
            isSwimming = false;

        [Header("Enable/Disable")] 
        [NonSerialized]
        public bool enableMultiJump = true,
            enableJumping = true,
            enableCrouching = true,
            enableCrouchToggle = false;

        // Gets componenets and sets base stats
        private void Start()
        {
            // Player Camera
            playerCamera = transform.GetChild(0);
            cameraProperties = playerCamera.GetComponent<PlayerCamera>();

            // Get components
            characterController = GetComponent<CharacterController>();
            forceModifier = GetComponent<PlayerForce>();
            playerSwim = GetComponent<PlayerSwim>();
            playerLadder = GetComponent<PlayerLadder>();

            // Init Variables
            initPlayerHeight = characterController.height;
            initCenter = characterController.center;

            crouchCamHeight = initPlayerHeight - crouchHeight;
            crouchCenter.y = characterController.height * 0.125f;
            initCamHeight = playerCamera.localPosition.y;
            initMoveSpeed = movementSpeed;
        }

        private void PlayerInput()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && enableJumping && !isSwimming)
            {
                Jump();
            }

            if (Input.GetButtonDown("Crouch") && !isCrouching)
            {
                Crouch();
                crouchOnLanding = !characterController.isGrounded;
            }
            else if (!Input.GetButton("Crouch") && isCrouching && !enableCrouchToggle)
            {
                Crouch();
                crouchOnLanding = !characterController.isGrounded;
            }

            if (Input.GetButtonUp("Crouch"))
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
            if (isClimbingLadder)
            {
                playerLadder.LadderVelocity();
            }
            else if (isSwimming)
            {
                playerSwim.SwimVelocityMovement();
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
            if (HeadCheck() && isJumping && !isFlying && !isSwimming)
            {
                gravity = -0.5f;
            }

            // Perform crouch upon landing
            if (characterController.isGrounded && crouchOnLanding)
            {
                Crouch();
                crouchOnLanding = false;
            }

            // Ensure movespeed is always initSpeed when not crouching
            if (!isCrouching)
            {
                movementSpeed = initMoveSpeed;
            }
        }

        // Main movement function
        private void PlayerVelocity()
        {
            // Gravity
            gravity += Time.deltaTime * worldGravity;
            inputVelocity = new Vector3(horizontal, 0, vertical);

            // Prevent excessive diagonal movement
            if (inputVelocity.sqrMagnitude > 1)
                inputVelocity = inputVelocity.normalized;

            // Finalization
            inputVelocity = transform.rotation * inputVelocity;
            inputVelocity *= movementSpeed;
            inputVelocity.y = gravity;
            characterController.Move(inputVelocity * Time.deltaTime);

            if (characterController.isGrounded)
            {
                canJumpInAir = true;
                isJumping = false;
                jumpCounter = 0;
                gravity = 0;
            }

            if (IsOnSlope())
            {
                float slopeStep = Time.deltaTime * 5;
                characterController.Move(Vector3.down * slopeStep);
            }
        }

        private void Jump()
        {
            // Remove a jump if player is in air
            if (!characterController.isGrounded && !isJumping && canJumpInAir)
            {
                jumpCounter += 1;
                canJumpInAir = false;
            }

            // Multi jump
            if (isJumping && maximumJumps != jumpCounter)
            {
                jumpCounter += 1;
            }

            // AddForce() will make isJumping true
            if (characterController.isGrounded || (jumpCounter != maximumJumps && enableMultiJump))
            {
                forceModifier.AddForce(jumpHeight);
            }
        }

        private void Crouch()
        {
            if (characterController.isGrounded && enableCrouching)
            {
                if (!isCrouching)
                {
                    StartCoroutine(CrouchRoutine());

                    if (!isFlying && !isJumping)
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
            float crouchParam = 0f;

            float crouchSpeed = 1f / crouchTransitionDuration;
            float currentHeight = characterController.height;
            Vector3 currentCenter = characterController.center;

            float desiredHeight = isCrouching ? initPlayerHeight : crouchHeight;
            Vector3 desiredCenter = isCrouching ? initCenter : crouchCenter;

            Vector3 camPos = playerCamera.localPosition;
            float camCurrentHeight = camPos.y;
            float camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

            while (crouchParam < 1f)
            {
                crouchParam += Time.deltaTime * crouchSpeed;

                characterController.height = Mathf.Lerp(currentHeight, desiredHeight, crouchParam);
                characterController.center = Vector3.Lerp(currentCenter, desiredCenter, crouchParam);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, crouchParam);
                playerCamera.localPosition = camPos;

                // Adjust player position for resize
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

        private IEnumerator AdjustSpeedRoutine(float rate, float newSpeed)
        {
            float elapsedTime = 0.0f;
            
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * rate;
                movementSpeed = Mathf.Lerp(movementSpeed, newSpeed, elapsedTime);

                yield return null;
            }
        }

        // True if object is detected above head
        public bool HeadCheck()
        {
            float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
            return Physics.SphereCast(transform.position, characterController.radius, Vector3.up,
                out RaycastHit hitInfo, rayLength);
        }

        private bool IsOnSlope()
        {
            if (isJumping)
                return false;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                if (hit.normal != Vector3.up)
                    return true;
            }

            return false;
        }

        // Buggy
        private void SlideOffSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            {
                Vector3 incomingVec = hit.point - transform.position;
                slopeDirection =  Vector3.Reflect(incomingVec, hit.normal);
            }

            if(slopeDirection.y <= -0.1f && !isFlying && !isJumping)
            {
                currentSlideSpeed += Time.deltaTime * slideRate;
                characterController.Move(Vector3.down * 5 * Time.deltaTime);
                characterController.Move(slopeDirection * currentSlideSpeed * Time.deltaTime);
                timedDelay = slideDelay;

                //enableJumping = false;
            }
            else
            {
                timedDelay -= Time.deltaTime;
                //enableJumping = true;
            }

            // Ensure all force is gone
            if (timedDelay <= 0)
            {
                slopeDirection = Vector3.zero;
                currentSlideSpeed = 0;
            }
        }

        private void RotateBodyWithView()
        {
            Vector3 bodyRot = new Vector3(0, playerCamera.eulerAngles.y, 0);
            transform.eulerAngles = bodyRot;
        }

        // Debugging
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * rayLength, .5f);
        }
    }
}
using System;
using System.Collections;
using UnityEngine;
using Managers;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Components
        
        [Header("Components")] 
        public CharacterController characterController;
        [NonSerialized] public PlayerCamera cameraProperties;
        private PlayerLadder playerLadder;
        private PlayerForce forceModifier;
        private PlayerSwim playerSwim;
        private Transform playerCamera;
        
        #endregion
        
        [Header("Input")] 
        [NonSerialized] public Vector3 inputVelocity;
        [NonSerialized] public float horizontal;
        [NonSerialized] public float vertical;

        [Header("Gravity")] 
        [SerializeField] private float worldGravity = Physics.gravity.y;
        public float gravity = 0;

        #region Settings

        [Header("Movement Speed Settings")] 
        public float movementSpeed = 7.0f;
        [SerializeField] private float crouchMovementSpeed = 2.8f;
        
        [Header("Jump Settings")] 
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private int maximumJumps = 2;
        private int jumpCounter;

        [Header("Crouch Settings")] 
        [SerializeField] private float crouchHeight = 1.4f;
        [SerializeField] private float crouchTransitionDuration = 0.25f;
        private float crouchDecelerateRate = 2.0f;
        private bool crouchOnLanding;
        private float crouchCamHeight;

        [Header("Slope Sliding")]
        [SerializeField] private float slideRate = 0.1f;
        private float currentSlideSpeed = 0.0f;
        
        #endregion
        
        [Header("Init Variables")] 
        private Vector3 initCenter;
        private float initMoveSpeed;
        private float initPlayerHeight;
        private float initCamHeight;
        private Vector3 crouchCenter;

        #region Conditions
        
        [Header("Player Conditions")]
        [NonSerialized] 
        public bool isClimbingLadder = false,
            isCrouching = false,
            isJumping = false,
            isFlying = false,
            isSwimming = false;

        #endregion
        
        [Header("Enable/Disable")] 
        [NonSerialized]
        public bool enableMultiJump = true,
            enableJumping = true,
            enableCrouching = true,
            enableCrouchToggle = false;

        // Gets components and sets base stats
        private void Start()
        {
            // Player Camera
            playerCamera = transform.GetChild(0);
            cameraProperties = playerCamera.GetComponent<PlayerCamera>();

            // Get components
            //characterController = GetComponent<CharacterController>();
            forceModifier = GetComponent<PlayerForce>();
            playerLadder = GetComponent<PlayerLadder>();
            playerSwim = GetComponent<PlayerSwim>();
            
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
            if (!GameState.isPaused)
            {
                PlayerInput();
            }
            
            PlayerMisc();
        }

        // Movement functions
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
                enableJumping = !isCrouching;
                PlayerVelocity();
                SlopeSliding();
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
        
        private void PlayerVelocity()
        {
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
                isJumping = false;
                jumpCounter = 0;
                gravity = 0;
            }

            // Downward force on unevenground
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                if (hit.normal != Vector3.up && !isJumping && !isFlying)
                {
                    float slopeStep = Time.deltaTime * 5;
                    characterController.Move(Vector3.down * slopeStep);
                }
            }
        }

        private void Jump()
        {
            // Remove a jump if player is in air
            if (!characterController.isGrounded && !isJumping)
            {
                jumpCounter += 1;
            }
            
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
                        StartCoroutine(AdjustMoveSpeed(crouchDecelerateRate, crouchMovementSpeed));

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

        private IEnumerator AdjustMoveSpeed(float rate, float newSpeed)
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
                out RaycastHit hitInfo, rayLength, ~LayerMask.GetMask("Ignore Player"));
        }

        private void SlopeSliding()
        {
            if (characterController.isGrounded && !isFlying)
            {
                Vector3 slopeDirection = Vector3.zero;
            
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height, ~0, QueryTriggerInteraction.Ignore))
                {
                    Vector3 temp = Vector3.Cross(hit.normal, hit.point - transform.position);
                    slopeDirection = Vector3.Cross(temp, hit.normal);
                }
            
                if(slopeDirection.y <= -0.45f)
                {
                    currentSlideSpeed += Time.deltaTime * slideRate;
                    characterController.Move(slopeDirection.normalized * currentSlideSpeed);
                    enableJumping = false;
                }
                else
                {
                    currentSlideSpeed = 0;
                    enableJumping = true;
                }
            }
        }

        private void RotateBodyWithView()
        {
            Vector3 bodyRot = new Vector3(0, playerCamera.eulerAngles.y, 0);
            transform.eulerAngles = bodyRot;
        }
    }
}
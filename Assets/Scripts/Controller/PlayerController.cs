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
        
        // Required
        public CharacterController characterController;
        
        // Jumping
        private PlayerForce forceModifier;
        
        // Depend on each other
        [NonSerialized] public PlayerCamera cameraProperties;
        private PlayerLadder playerLadder;
        private PlayerSwim playerSwim;
        
        #endregion

        #region Input Vars

        private Vector3 inputVelocity;
        private float horizontal;
        private float vertical;
        
        [Header("Gravity")]
        // Recommended -14 worldGravity
        public float worldGravity = Physics.gravity.y;
        public float gravity;

        #endregion

        #region Init Vars

        private Transform cameraTransform;
        private Vector3 initCenter;
        private float initMoveSpeed;
        private float initPlayerHeight;
        private float initCamHeight;
        private Vector3 crouchCenter;

        #endregion
        
        #region Conditions
        
        [NonSerialized]
        public bool
            isJumping,
            isCrouching,
            isFlying,
            isClimbingLadder,
            isSwimming;

        // Var is updated by other scripts
        private bool enableJumping = true;
        
        #endregion
        
        #region Settings

        [Header("Movement")] 
        public float movementSpeed = 7.0f;
        public float crouchMovementSpeed = 2.8f;
        public bool enableCrouching = true;
        public bool enableCrouchToggle = false;
        
        [Header("Jump")] 
        public float jumpHeight = 5f;
        public int maximumJumps = 1;
        private int jumpCounter;

        [Header("Crouch")] 
        [SerializeField] private float crouchHeight = 1.4f;
        [SerializeField] private float crouchTransitionDuration = 0.1f;
        private float crouchCamHeight;

        [Header("Slope Sliding")]
        private const float SlideRate = 0.1f;
        private float currentSlideSpeed;
        
        #endregion
        
        private void Start()
        {
            cameraTransform = transform.GetChild(0);
            
            // Movement components
            forceModifier = GetComponent<PlayerForce>();
            playerLadder = GetComponent<PlayerLadder>();
            playerSwim = GetComponent<PlayerSwim>();
            cameraProperties = cameraTransform.GetComponent<PlayerCamera>();

            // Init Variables
            initPlayerHeight = characterController.height;
            initCenter = characterController.center;

            crouchCamHeight = initPlayerHeight - crouchHeight;
            crouchCenter.y = characterController.height * 0.125f;
            initCamHeight = cameraTransform.localPosition.y;
            initMoveSpeed = movementSpeed;
        }

        private void Update()
        {
            if (!GameState.isPaused)
            {
                PlayerInput();
            }
        }
        
        private void PlayerInput()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && enableJumping && !isSwimming)
            {
                Jump();
            }

            if (enableCrouching)
            {
                if (Input.GetButtonDown("Crouch") && enableCrouchToggle)
                {
                    Crouch();
                }
                else if (!enableCrouchToggle)
                {
                    if (Input.GetButtonDown("Crouch"))
                    {
                        Crouch();
                    }
                    else if (Input.GetButtonUp("Crouch"))
                    {
                        Crouch();
                    }
                }
            }
        }

        // Movement functions
        private void LateUpdate()
        {
            if (isClimbingLadder)
            {
                playerLadder.LadderVelocity();
                enableJumping = false;
            }
            else if (isSwimming)
            {
                playerSwim.SwimVelocityMovement();
                enableJumping = false;
            }
            else
            {
                enableJumping = !isCrouching;
                PlayerVelocity();
                SlopeSliding();
            }

            RotateBodyWithView();
        }

        private void PlayerVelocity()
        {
            gravity += Time.deltaTime * worldGravity;
            inputVelocity = new Vector3(horizontal, 0, vertical);

            // Prevent player from excessive floating when jumping
            if (HeadCheck() && isJumping && !isFlying && !isSwimming)
                gravity = -0.5f;

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
            
            if (characterController.isGrounded || jumpCounter != maximumJumps)
            {
                forceModifier.AddYForce(jumpHeight);
            }
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
                    currentSlideSpeed += Time.deltaTime * SlideRate;
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
        
        private void Crouch()
        {
            if (characterController.isGrounded)
            {
                if (!isCrouching)
                {
                    StartCoroutine(CrouchRoutine());
                    movementSpeed = crouchMovementSpeed;

                    isCrouching = true;
                }
                else if (!HeadCheck())
                {
                    StartCoroutine(CrouchRoutine());
                    movementSpeed = initMoveSpeed;

                    isCrouching = false;
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
            float crouchParam = 0f;

            float crouchSpeed = 1f / crouchTransitionDuration;
            float currentHeight = characterController.height;
            Vector3 currentCenter = characterController.center;

            float desiredHeight = isCrouching ? initPlayerHeight : crouchHeight;
            Vector3 desiredCenter = isCrouching ? initCenter : crouchCenter;

            Vector3 camPos = cameraTransform.localPosition;
            float camCurrentHeight = camPos.y;
            float camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

            while (crouchParam < 1f)
            {
                crouchParam += Time.deltaTime * crouchSpeed;

                characterController.height = Mathf.Lerp(currentHeight, desiredHeight, crouchParam);
                characterController.center = Vector3.Lerp(currentCenter, desiredCenter, crouchParam);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, crouchParam);
                cameraTransform.localPosition = camPos;

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

        private void RotateBodyWithView()
        {
            Vector3 bodyRot = new Vector3(0, cameraTransform.eulerAngles.y, 0);
            transform.eulerAngles = bodyRot;
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
            float rayLength = isCrouching ? crouchCenter.y + 0.75f : .6f;
            return Physics.SphereCast(transform.position, characterController.radius, Vector3.up,
                out RaycastHit hitInfo, rayLength, ~LayerMask.GetMask("Ignore Player"));
        }
    }
}
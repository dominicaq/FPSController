using System;
using System.Collections;
using UnityEngine;

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
            isSliding,
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
            // Movement components
            forceModifier = GetComponent<PlayerForce>();
            playerLadder = GetComponent<PlayerLadder>();
            playerSwim = GetComponent<PlayerSwim>();
            cameraTransform = transform.GetChild(0);
            cameraProperties = cameraTransform.GetComponent<PlayerCamera>();
            
            // Init Variables
            initPlayerHeight = characterController.height;
            initCenter = characterController.center;

            crouchCamHeight = initPlayerHeight - crouchHeight;
            crouchCenter.y = characterController.height * 0.125f;
            initCamHeight = cameraTransform.localPosition.y;
            initMoveSpeed = movementSpeed;
        }

        private void PlayerInput()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && enableJumping)
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
                PlayerInput();
                PlayerVelocity();
                SlopeSliding();
            }

            Vector3 bodyRot = new Vector3(0, cameraTransform.eulerAngles.y, 0);
            transform.eulerAngles = bodyRot;
        }

        private void PlayerVelocity()
        {
            gravity += Time.deltaTime * worldGravity;
            inputVelocity = new Vector3(horizontal, 0, vertical);
            
            if (inputVelocity.sqrMagnitude > 1)
                inputVelocity = inputVelocity.normalized;
            
            // Prevent player from excessive floating
            if (HeadCheck() && isJumping)
                gravity = -Mathf.Abs(gravity);
            
            inputVelocity = (transform.rotation * inputVelocity) * movementSpeed;
            inputVelocity.y = gravity;
            characterController.Move(inputVelocity * Time.deltaTime);

            if (characterController.isGrounded)
            {
                isJumping = false;
                jumpCounter = 0;
                gravity = 0;
            }

            // Downward force on unevenground
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height * .75f))
            {
                if (hit.normal != Vector3.up && !isJumping)
                {
                    float slopeStep = Time.deltaTime * 5;
                    characterController.Move(Vector3.down * slopeStep);
                }
            }
        }

        private void Jump()
        {
            jumpCounter += 1;

            if (jumpCounter < maximumJumps + 1)
            {
                if(!characterController.isGrounded && maximumJumps == 1 || isSliding)
                    return;
                
                forceModifier.AddYForce(jumpHeight);
            }
        }

        private void SlopeSliding()
        {
            if (characterController.isGrounded)
            {
                Vector3 slopeDirection = Vector3.zero;
            
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height, ~0, QueryTriggerInteraction.Ignore))
                {
                    Vector3 temp = Vector3.Cross(hit.normal, hit.point - transform.position);
                    slopeDirection = Vector3.Cross(temp, hit.normal);
                }
            
                if(slopeDirection.y <= -0.45f)
                {
                    isSliding = true;
                    currentSlideSpeed += Time.deltaTime * SlideRate;
                    characterController.Move(slopeDirection.normalized * currentSlideSpeed);
                }
            }
            else
            {
                isSliding = false;
                currentSlideSpeed = 0;
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
using UnityEngine;

namespace Controller
{
    // Incomplete
    [RequireComponent(typeof(PlayerController))]
    public class PlayerSwim : MonoBehaviour
    {
        [Header("Properties")] [SerializeField] [Range(1, 10)]
        private float swimMovementSpeed = 7.0f;

        private Vector3 swimVelocity;
        public float ascendRate = 10;
        public float descendRate = 5;

        [Header("Components")] private PlayerController playerController;
        private CharacterController charController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            charController = GetComponent<CharacterController>();

            if (!playerController || !charController)
                enabled = false;
        }

        // Early version of swimming
        public void SwimVelocityMovement()
        {
            playerController.enableJumping = false;
            playerController.isJumping = false;
            playerController.isFlying = false;

            swimVelocity = new Vector3(playerController.horizontal, 0, playerController.vertical);
            float buoyancy = playerController.gravity;

            // Prevent excessive diagonal movement
            if (swimVelocity.sqrMagnitude > 1)
                swimVelocity = swimVelocity.normalized;

            if (Input.GetButton("Jump"))
            {
                buoyancy += ascendRate * Time.deltaTime;

                if (buoyancy >= 3)
                    buoyancy = 3;
            }

            if (buoyancy < -1 && !charController.isGrounded)
            {
                buoyancy += -buoyancy * Time.deltaTime;
            }
            else
            {
                buoyancy -= descendRate * Time.deltaTime;

                if (buoyancy <= -1)
                    buoyancy = -1;
            }

            // Finalization
            playerController.gravity = buoyancy;
            swimVelocity = transform.rotation * swimVelocity;
            swimVelocity *= swimMovementSpeed;
            swimVelocity.y = buoyancy;
            charController.Move(swimVelocity * Time.deltaTime);
        }

        private bool HeadWaterCheck()
        {
            float rayLength = 0.6f;
            if (Physics.SphereCast(transform.position, charController.radius, Vector3.up, out RaycastHit hitInfo, rayLength))
            {
                //Debug.Log(hitInfo.collider.isTrigger);
            }

            return false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Water"))
                playerController.isSwimming = true;

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water") && !HeadWaterCheck() && playerController.isSwimming)
                playerController.isSwimming = false;
        }
    }
}
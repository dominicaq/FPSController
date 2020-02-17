using UnityEngine;

namespace Controller
{
    // Incomplete
    [RequireComponent(typeof(PlayerController))]
    public class PlayerSwim : MonoBehaviour
    {
        [Header("Properties")]
        public float swimMovementSpeed = 7.0f;
        private Vector3 swimVelocity;
        public float ascendRate = 10;
        public float descendRate = 5;

        [Header("Input")] 
        private float horizontal;
        private float vertical;
        
        [Header("Components")] 
        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }
        
        /// <summary> Gives player control of their y axis position a trigger </summary>
        public void SwimVelocityMovement()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            
            swimVelocity = new Vector3(horizontal, 0, vertical);
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

            if (buoyancy < -1 && !playerController.characterController.isGrounded)
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
            playerController.characterController.Move(swimVelocity * Time.deltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Water"))
                playerController.isSwimming = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water") && playerController.isSwimming)
                playerController.isSwimming = false;
        }
    }
}
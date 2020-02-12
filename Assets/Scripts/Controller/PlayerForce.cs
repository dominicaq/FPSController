using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerForce : MonoBehaviour
    {
        [Header("Force Vector")] 
        public Vector3 velocity = Vector3.zero;

        [Header("Force Conditions")] [SerializeField]
        private bool enableVelocityForce = true;
        [SerializeField] private bool collidedWithWall = false;

        // Scripts
        private CharacterController playerCC;
        private PlayerController playerController;

        private void Awake()
        {
            playerCC = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            if (enableVelocityForce)
            {
                float decelerate = 1;
                if (collidedWithWall && !playerCC.isGrounded)
                    decelerate = 2.5f;

                // Decelerate on ground or head bump
                if (playerCC.isGrounded || playerController.HeadCheck())
                {
                    decelerate = 5f;
                    velocity.y = 0;
                }

                velocity = Vector3.Lerp(velocity, Vector3.zero, decelerate * Time.deltaTime);
                
                // Finalization
                if (velocity.magnitude > 1 || velocity.magnitude < -1)
                {
                    playerCC.Move(velocity * Time.deltaTime);
                }
                else if (playerCC.isGrounded)
                {
                    velocity = Vector3.zero;
                    playerController.isFlying = false;
                    collidedWithWall = false;
                }
            }
        }

        // Detect if we hit a wall
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            float rayLength = 0.5f;
            
            // Sphere cast or ray cast and is flying
            bool isValid = (playerController.isFlying && 
                            Physics.SphereCast(transform.position, playerCC.radius / 1.5f,velocity, out RaycastHit hitInfo, rayLength) ||
                            Physics.Raycast(transform.position, velocity));

            if (isValid)
            {
                collidedWithWall = true;
            }
        }

        // Vector force
        public void AddForce(Vector3 dir)
        {
            playerController.isFlying = true;
            collidedWithWall = false;

            playerController.gravity = 0;
            velocity -= Vector3.Reflect(dir, Vector3.zero);
        }

        // Y Axis force
        public void AddForce(float upwardForce)
        {
            playerController.isJumping = true;
            playerController.gravity = upwardForce;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + velocity * 0.1f);
            Gizmos.DrawWireSphere(transform.position + velocity * 0.1f, 0.5f / 1.5f);
        }
    }
}
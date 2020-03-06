using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerForce : MonoBehaviour
    {
        public Vector3 velocity = Vector3.zero;
        private bool collidedWithWall;
        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            if (velocity != Vector3.zero)
            {
                float decelerate = 1;
                if (collidedWithWall && !playerController.characterController.isGrounded)
                    decelerate = 2.5f;

                // Decelerate on ground or head bump
                if (playerController.characterController.isGrounded || playerController.HeadCheck())
                {
                    decelerate = 5f;
                    velocity.y = 0;
                }

                velocity = Vector3.Lerp(velocity, Vector3.zero, decelerate * Time.deltaTime);
                
                if (velocity.magnitude > 1 || velocity.magnitude < -1)
                {
                    playerController.characterController.Move(velocity * Time.deltaTime);
                }
                else if (playerController.characterController.isGrounded)
                {
                    velocity = Vector3.zero;
                    playerController.isJumping = false;
                    collidedWithWall = false;
                }
            }
        }

        // Detect if we hit a wall
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            float rayLength = 0.5f;
            
            // Sphere cast or ray cast and is flying
            bool isValid = (playerController.isJumping && 
                            Physics.SphereCast(transform.position, playerController.characterController.radius / 1.5f,velocity, out RaycastHit hitInfo, rayLength) ||
                            Physics.Raycast(transform.position, velocity));

            if (isValid)
            {
                collidedWithWall = true;
            }
        }
        
        /// <summary> Pushes player in given direction </summary>
        /// <param name="dir"></param>
        public void AddForce(Vector3 dir)
        {
            playerController.isJumping = true;
            collidedWithWall = false;

            playerController.gravity = 0;
            velocity -= Vector3.Reflect(dir, Vector3.zero);
        }
        
        /// <summary> Makes playerController.isJumping true when called </summary>
        /// <param name="upwardForce"></param>
        public void AddYForce(float upwardForce)
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
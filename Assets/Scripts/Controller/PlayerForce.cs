using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerForce : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    private bool collidedWithWall;
    private PlayerMovementManager m_movementManager;
    private void Start()
    {
        m_movementManager = GetComponent<PlayerMovementManager>();
    }

    private void LateUpdate()
    {
        if (velocity != Vector3.zero)
        {
            float decelerate = 1;
            if (collidedWithWall && !m_movementManager.controller.isGrounded)
                decelerate = 2.5f;

            // Decelerate on ground or head bump
            if (m_movementManager.controller.isGrounded || m_movementManager.playerController.HeadCheck())
            {
                decelerate = 5f;
                velocity.y = 0;
            }

            velocity = Vector3.Lerp(velocity, Vector3.zero, decelerate * Time.deltaTime);
            
            if (velocity.magnitude > 1 || velocity.magnitude < -1)
            {
                m_movementManager.controller.Move(velocity * Time.deltaTime);
            }
            else if (m_movementManager.controller.isGrounded)
            {
                velocity = Vector3.zero;
                m_movementManager.isJumping = false;
                collidedWithWall = false;
            }
        }
    }

    // Detect if we hit a wall
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float rayLength = 0.5f;
        
        // Sphere cast or ray cast and is flying
        bool isValid = (m_movementManager.isJumping && 
                        Physics.SphereCast(transform.position, m_movementManager.controller.radius / 1.5f,velocity, out RaycastHit hitInfo, rayLength) ||
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
        m_movementManager.isJumping = true;
        collidedWithWall = false;

        m_movementManager.playerController.gravity = 0;
        velocity -= Vector3.Reflect(dir, Vector3.zero);
    }
    
    /// <summary> Makes playerController.isJumping true when called </summary>
    /// <param name="upwardForce"></param>
    public void AddYForce(float upwardForce)
    {
        m_movementManager.isJumping = true;
        m_movementManager.playerController.gravity = upwardForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity * 0.1f);
        Gizmos.DrawWireSphere(transform.position + velocity * 0.1f, 0.5f / 1.5f);
    }
}
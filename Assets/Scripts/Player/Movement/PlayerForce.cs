using UnityEngine;

[RequireComponent(typeof(BaseController))]
public class PlayerForce : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    private bool m_Collided;
    private PlayerStateManager stateManager;
    private CharacterController m_CharacterController;

    private void Awake()
    {
        stateManager          = GetComponent<PlayerStateManager>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        if (velocity != Vector3.zero)
        {
            float decelerate = 1;

            if (m_Collided && ! m_CharacterController.isGrounded)
                decelerate = 2.5f;


            // Decelerate on ground or head bump
            if(stateManager.currentController.ObjectAbove() || m_CharacterController.isGrounded) {
                velocity.y = 0;
                decelerate = 5f;
            }

            velocity = Vector3.Lerp(velocity, Vector3.zero, decelerate * Time.deltaTime);
            
            if (velocity.magnitude > 1 || velocity.magnitude < -1)
                m_CharacterController.Move(velocity * Time.deltaTime);
            else if (m_CharacterController.isGrounded)
            {
                velocity = Vector3.zero;
                m_Collided = false;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Sphere cast or ray cast and is flying
        m_Collided = Physics.SphereCast(transform.position, m_CharacterController.radius / 1.5f, velocity, out RaycastHit hitInfo, 0.5f) ||
                     Physics.Raycast(transform.position, velocity);
    }
    
    public void AddForce(Vector3 dir)
    {
        m_Collided = false;

        stateManager.currentController.currentGravity = 0;
        velocity -= Vector3.Reflect(dir, Vector3.zero);
    }
    
    public void AddYForce(float upwardForce)
    {
        stateManager.currentController.currentGravity = upwardForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity * 0.1f);
        Gizmos.DrawWireSphere(transform.position + velocity * 0.1f, 0.5f / 1.5f);
    }
}
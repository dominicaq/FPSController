using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerForce : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    private bool m_Collided;
    private bool m_isKnockback;
    private CharacterController m_Controller;
    private PlayerController m_PlayerControler;

    private void Start()
    {
        m_PlayerControler = GetComponent<PlayerController>();
        m_Controller      = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        if (velocity != Vector3.zero)
        {
            float decelerate = 1;

            if (m_Collided && !m_Controller.isGrounded)
                decelerate = 2.5f;

            if(m_Controller.isGrounded)
                decelerate = 5.0f;

            // Decelerate on ground or head bump
            if(m_PlayerControler.HeadCheck() || (m_Controller.isGrounded && !m_isKnockback))
            {
                velocity.y = 0;
                decelerate = 5f;
            }
                
            velocity = Vector3.Lerp(velocity, Vector3.zero, decelerate * Time.deltaTime);
            
            if (velocity.magnitude > 1 || velocity.magnitude < -1)
            {
                m_Controller.Move(velocity * Time.deltaTime);
            }
            else if (m_Controller.isGrounded)
            {
                velocity = Vector3.zero;
                m_Collided = false;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float rayLength = 0.5f;
        
        // Sphere cast or ray cast and is flying
        m_Collided = Physics.SphereCast(transform.position, m_Controller.radius / 1.5f, velocity, out RaycastHit hitInfo, rayLength) ||
                     Physics.Raycast(transform.position, velocity);
    }
    
    public void AddForce(Vector3 dir, bool isKnockback)
    {
        m_isKnockback = isKnockback;
        m_Collided = false;

        m_PlayerControler.gravity = 0;
        velocity -= Vector3.Reflect(dir, Vector3.zero);
    }
    
    public void AddYForce(float upwardForce)
    {
        m_PlayerControler.isJumping = true;
        m_PlayerControler.gravity = upwardForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity * 0.1f);
        Gizmos.DrawWireSphere(transform.position + velocity * 0.1f, 0.5f / 1.5f);
    }
}
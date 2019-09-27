using UnityEngine;

public class PlayerForce : MonoBehaviour
{
    [Header("Character Force Conditions")]
    [SerializeField] private bool enablevelocityForce = true;
    [SerializeField] private bool collidedWithWall = false;

    [Header("Force Vector")]
    public Vector3 velocity = Vector3.zero;

    // Scripts
    private CharacterController playerCC;
    private PlayerController playerMovement;

    private void Awake()
    {
        playerCC = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        if(enablevelocityForce)
        {
            if(collidedWithWall && !playerCC.isGrounded)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 2.5f);
            }
            
            PlayerImpulse();
        }
    }
    
    public void AddForce(Vector3 dir)
    {
        playerMovement.isFlying = true;
        collidedWithWall = false;

        // Additional strength if player is jumping
        if(playerMovement.isCrouching && playerMovement.isJumping)
        {
            dir.y = dir.y * 2.5f;
        }

        playerMovement.gravity = 0;
        velocity -= Vector3.Reflect(dir, Vector3.zero);
    }    

    public void AddForce(float force)
    {
        playerMovement.isJumping = true;
        playerMovement.gravity = Mathf.Sqrt(force);
    }

    private void PlayerImpulse()
    {
        if (velocity.magnitude > .3f) 
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime);

            // Decelerate on ground
            if(playerCC.isGrounded)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 5f );
                velocity.y = 0;

                // Decelerate faster if player input detected
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !playerCC.isGrounded)
                {
                    velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 15f );
                    velocity.z = Mathf.Lerp(velocity.z, 0, Time.deltaTime * 15f );
                }
            }

            // Prevent velocity from pushing player into ceiling
            if(playerMovement.HeadCheck())
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 30f);
            }
        }

        // Finalization
        if(velocity.magnitude > 1 || velocity.magnitude < -1)
        {
            playerCC.Move(velocity * Time.deltaTime);
        }
        else if (playerCC.isGrounded)
        {
            velocity = Vector3.zero;
            playerMovement.isFlying = false;
            collidedWithWall = false;
        }
    }

    // DON'T USE MOVEMENT FUNCTIONS INSIDE, STACKOVERFLOW ERROR WILL OCCUR
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If player hits a wall while being launched, nullify velocity immediately
        float rayLength = 0.5f;

        // Sphere cast or ray cast and is flying
        bool isValid = (Physics.SphereCast(transform.position, playerCC.radius / 1.5f, velocity, out RaycastHit hitInfo, rayLength) ||
        Physics.Raycast(transform.position, velocity)) 
        && playerMovement.isFlying;

        if(isValid)
        {
            collidedWithWall = true;
        }
    }

    private void OnDrawGizmosSelected()
    { 
        float gizmoRayLength = 0.1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + velocity * gizmoRayLength);
        Gizmos.DrawWireSphere(transform.position + velocity * gizmoRayLength, 0.5f / 1.5f);
    }
}

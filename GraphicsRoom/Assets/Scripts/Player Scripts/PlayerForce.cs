using UnityEngine;

public class PlayerForce : MonoBehaviour
{
    [Header("Character Force Conditions")]
    [SerializeField] private bool enableImpactForce = true;
    [SerializeField] private bool collidedWithWall = false;

    [Header("Force Vector")]
    public Vector3 impact = Vector3.zero;

    // Scripts
    private CharacterController playerCC;
    private PlayerController playerMovement;

    private void Awake()
    {
        playerCC = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(enableImpactForce)
        {
            if(collidedWithWall && !playerCC.isGrounded)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 2.5f);
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
            dir.y = dir.y * 1.25f;
        }

        playerMovement.velocity = 0;
        impact -= Vector3.Reflect(dir, Vector3.zero);
    }    

    public void AddForce(float force)
    {
        playerMovement.isJumping = true;
        playerMovement.velocity = Mathf.Sqrt(force);
    }

    private void PlayerImpulse()
    {
        if (impact.magnitude > .3f) 
        {
            impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);

            // Decelerate on ground
            if(playerCC.isGrounded)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 5f );
                impact.y = 0;

                // Decelerate faster if player input detected
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 10f );
                }
            }

            // Prevent impact from pushing player into ceiling
            if(playerMovement.HeadCheck())
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 30f);
            }
        }
            
        // Finalization
        if(impact.magnitude > 1 || impact.magnitude < -1)
        {
            playerCC.Move(impact * Time.deltaTime);
        }
        else if (playerCC.isGrounded)
        {
            impact = Vector3.zero;
            playerMovement.isFlying = false;
            collidedWithWall = false;
        }
    }

    // DON'T USE MOVEMENT FUNCTIONS INSIDE, STACKOVERFLOW ERROR WILL OCCUR
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If player hits a wall while being launched, nullify impact immediately
        float rayLength = 0.5f;

        // Sphere cast or ray cast and is flying
        bool isValid = (Physics.SphereCast(transform.position, playerCC.radius / 1.5f, impact, out RaycastHit hitInfo, rayLength) ||
        Physics.Raycast(transform.position, impact)) 
        && playerMovement.isFlying;

        if(isValid)
        {
            collidedWithWall = true;
        }
    }

    void OnDrawGizmos()
    { 
        // Draw a yellow sphere at the transform's position
        float gizmoRayLength = 0.1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + impact * gizmoRayLength);
        Gizmos.DrawWireSphere(transform.position + impact * gizmoRayLength, 0.5f / 1.5f);
    }
}

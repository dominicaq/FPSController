using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForce : MonoBehaviour
{
    [Header("Character Force")]
    public bool enableImpactForce = true;
    public bool isFlying = false;
    public bool hitWall = false;

    [Header("Force Vector")]
    public Vector3 impact = Vector3.zero;
    // Necessary for collision if statement
    private Vector3 tempImpact = Vector3.zero;

    // Scripts
    private CharacterController playerCC;
    private PlayerController playerMovement;
    void Awake()
    {
        playerCC = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enableImpactForce)
        {
            if(hitWall && !playerCC.isGrounded)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 10f);
            }
            
            PlayerImpulse();
        }
    }
    
    public void AddForce(Vector3 dir)
    {
        isFlying = true;
        // Additional strength if player is jumping
        if(playerMovement.isCrouching && playerMovement.isJumping)
        {
            dir.y = dir.y * 1.25f;
        }

        playerMovement.velocity = 0;
        impact -= Vector3.Reflect(dir, Vector3.zero);
        tempImpact = impact;
    }    

    public void AddUpwardForce(float force)
    {
        playerMovement.velocity += Mathf.Sqrt(force);
    }

    private void PlayerImpulse()
    {
        if (impact.magnitude != 0) 
        {
            impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);

            // Decelerate
            if(playerCC.isGrounded)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 5f );
                impact.y = 0;

                // Decelerate faster if player input detected
                if ((playerMovement.horizontal != 0 || playerMovement.vertical != 0))
                {
                    impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 10f );
                }
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
            isFlying = false;
            hitWall = false;
        }
    }

    // DON'T USE MOVEMENT FUNCTIONS INSIDE, STACKOVERFLOW ERROR WILL OCCUR
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If player hits a wall while being launched, nullify impact immediately
        float rayLength = 0.5f;
        if(Physics.SphereCast(transform.position, playerCC.radius / 1.5f, tempImpact, out RaycastHit hitInfo, rayLength) && isFlying)
        {
            tempImpact = Vector3.zero;
            hitWall = true;
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

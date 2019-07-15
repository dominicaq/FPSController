using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForce : MonoBehaviour
{
    [Header("Character Force")]
    public bool enableImpactForce = true;
    private bool isFlying = false;
    private bool hitWall = false;

    [Header("Impact Vectors")]
    public Vector3 impact = Vector3.zero;
    // Necessary for collision if statement
    private Vector3 tempImpact = Vector3.zero;

    // Scripts
    private CharacterController playerCC;
    private PlayerController playerMovement;

    [Header("Debug")]
    public bool enableDebugging = false;

    void Start()
    {
        playerCC = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enableDebugging)
            Debug.DrawRay(transform.position, impact, Color.green);

        if(enableImpactForce)
        {
            playerImpulse();

            if(hitWall)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 15f);
                playerCC.Move(Vector3.down * Time.deltaTime);
                if(playerCC.isGrounded)
                {
                    hitWall = false;
                }
            }
        }
    }
    
    public void AddForce(Vector3 dir)
    {
        isFlying = true;

        // Double strength if player is jumping
        if(playerMovement.isJumping)
        {
            dir.y = dir.y * 1.5f;
        }

        impact -= Vector3.Reflect(dir, Vector3.zero);
        tempImpact = impact;
    }    

    public void AddUpwardForce(float force)
    {
        playerMovement.velocity += Mathf.Sqrt(2 * force);
    }

    private void playerImpulse()
    {
        if (impact.magnitude > .3f) 
        {
            impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);
        }

        // Decelerate
        if(playerCC.isGrounded)
        {
            impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 5f );
            impact.y = 0;

            // Decelerate faster if player input detected
            if (playerMovement.horizontal != 0 || playerMovement.vertical != 0)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * 10f );
            }
        }

        // Finalization
        playerCC.Move(impact * Time.deltaTime);
    }

    // DON'T USE MOVEMENT FUNCTIONS INSIDE, STACKOVERFLOW ERROR WILL OCCUR
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If player hits a wall while being launched, nullify impact immediately
        if(Physics.Raycast(transform.position, tempImpact * -1, playerCC.radius) && !playerMovement.isJumping && isFlying)
        {
            tempImpact = Vector3.zero;
            hitWall = true;
            isFlying = false;
        }
    }
}

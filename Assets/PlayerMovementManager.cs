using System;
using UnityEngine;

/*
    If player movement needs to be modified drastically, switch it here
*/
public class PlayerMovementManager : MonoBehaviour
{
    public float controlModifier = 1.0f;

    #region Componenets

    [NonSerialized] public PlayerCamera playerCamera;
    [NonSerialized] public PlayerController playerController;
    [NonSerialized] public CharacterController controller;
    
    [NonSerialized] public PlayerForce playerForce;
    [NonSerialized] public PlayerLadder playerLadder;
    [NonSerialized] public PlayerSwim playerSwim;
    
    #endregion

    #region Conditions

    public bool
        isJumping,
        isCrouching,
        isSliding,
        isClimbingLadder,
        isSwimming,
        isCharging;

    #endregion

    void Start()
    {
        playerCamera = transform.GetChild(0).GetComponent<PlayerCamera>();
        playerController = GetComponent<PlayerController>();
        controller = playerController.characterController;

        playerForce = GetComponent<PlayerForce>();
        playerLadder = GetComponent<PlayerLadder>();
        playerSwim = GetComponent<PlayerSwim>();
    }

    void LateUpdate()
    {
        if (isClimbingLadder)
        {
            playerLadder.LadderVelocity();
        }
        else if (isSwimming)
        {
            playerSwim.SwimVelocity();
        }
        else
        {
            playerController.enableJumping = !isCrouching;

            if(playerController.enableInput)
                playerController.PlayerInput();

            playerController.PlayerVelocity(controlModifier);
            playerController.SlopeSliding();
        }
    }
}

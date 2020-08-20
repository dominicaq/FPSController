using System;
using UnityEngine;
using PlayerStates;

namespace PlayerStates
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Crouching,
        Ladder,
        Swimming,
        Charging
    };
}

public class PlayerStateManager : MonoBehaviour
{
    #region Componenets

    [NonSerialized] public PlayerController playerController;
    [NonSerialized] public PlayerLadder playerLadder;
    [NonSerialized] public PlayerSwim playerSwim;
    [NonSerialized] public PlayerCharge playerCharge;
    
    #endregion

    #region Movement
    public float controlModifier = 1.0f;

    public PlayerState currentState;

    #endregion

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerLadder     = GetComponent<PlayerLadder>();
        playerSwim       = GetComponent<PlayerSwim>();
        playerCharge     = GetComponent<PlayerCharge>();
    }

    void LateUpdate()
    {
        // Rework to statemachine, then transfer it over to when I make enemies
        if(!playerController.enableInput)
            return;

        switch(currentState)
        {
            case PlayerState.Ladder :
            {
                playerLadder.LadderVelocity();
                break;
            }
            case PlayerState.Swimming :
            {
                playerSwim.SwimVelocity();
                break;
            }
            default :
            {
                playerController.PlayerVelocity(controlModifier);
                playerController.SlopeSliding();
                playerController.PlayerInput();
                States();
                break;
            }       
        }
    }

    private void States()
    {
        if(playerCharge.isCharging)
        {
            currentState = PlayerState.Charging;
            return;
        }

        if(playerController.isJumping)
        {
            currentState = PlayerState.Jumping;
            return;
        }

        if(playerController.isCrouching)
        {
            currentState = PlayerState.Crouching;
            return;
        }

        if(playerController.characterController.velocity.sqrMagnitude != 0)
        {
            currentState = PlayerState.Walking;
            return;
        }
            
        currentState = PlayerState.Idle;
    }
}

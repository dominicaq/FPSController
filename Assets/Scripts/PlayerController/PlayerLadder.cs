using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerLadder : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] [Range(0, 90)] private float ladderAngle = 5.0f;

    [Header("Input")]
    private float horizontal;
    private float vertical;
    
    [Header("Components")]
    private PlayerController playerController;
    private CharacterController controller;
    
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (playerController.isClimbingLadder)
        {
            horizontal = playerController.horizontal;
            vertical = playerController.vertical;
        }
    }
    
    public void LadderVelocity()
    {
        playerController.gravity = 0;
        playerController.enableJumping  = false;

        Vector3 ladderVelocity;
        if(!controller.isGrounded)
        {
            ladderVelocity = new Vector3 (horizontal,vertical,0);
        }
        else
        {
            ladderVelocity = new Vector3(horizontal, vertical, vertical);
        }

        if (playerController.cameraProperties.IsLookingDown(ladderAngle))
            ladderVelocity.y = -vertical;

        float moveRate = (playerController.cameraProperties.GetPitch() * 0.1f);
        ladderVelocity *= moveRate;

        Vector3 ladderDirection= transform.rotation * ladderVelocity;
        controller.Move(ladderDirection * Time.deltaTime);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(!playerController.isSwimming && other.CompareTag("Ladder"))
            playerController.isClimbingLadder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(playerController.isClimbingLadder)
            playerController.isClimbingLadder = false;
    }
}

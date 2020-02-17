using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerLadder : MonoBehaviour
    {
        [Header("Properties")] 
        [SerializeField] [Range(-90, 90)]
        private float ladderAngle = 15.0f;

        [Header("Input")] 
        private float horizontal;
        private float vertical;
        
        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        /// <summary> Moves player up and down y axis </summary>
        public void LadderVelocity()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            
            playerController.gravity = 0;

            Vector3 ladderVelocity;
            if (!playerController.characterController.isGrounded)
            {
                ladderVelocity = new Vector3(horizontal, vertical, 0);
            }
            else
            {
                ladderVelocity = new Vector3(horizontal, vertical, vertical);
            }

            if (playerController.cameraProperties.IsLookingDown(ladderAngle))
                ladderVelocity.y = -vertical;

            float moveRate =  Mathf.Clamp(Mathf.Abs(playerController.cameraProperties.GetPitch()), 0, 2);
            ladderVelocity *= moveRate * 2;
            Vector3 ladderDirection = transform.rotation * ladderVelocity;
            playerController.characterController.Move(ladderDirection * Time.deltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!playerController.isSwimming && other.CompareTag("Ladder"))
                playerController.isClimbingLadder = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (playerController.isClimbingLadder)
                playerController.isClimbingLadder = false;
        }
    }
}
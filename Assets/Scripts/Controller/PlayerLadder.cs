using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerLadder : MonoBehaviour
{
    [Header("Properties")] 
    [Range(-90, 90)] public float ladderAngle = 15.0f;

    [Header("Input")] 
    private float m_horizontal;
    private float m_vertical;
    
    private PlayerMovementManager m_movementManager;

    private void Start()
    {
        m_movementManager = GetComponent<PlayerMovementManager>();
    }

    /// <summary> Moves player up and down y axis </summary>
    public void LadderVelocity()
    {
        m_horizontal = Input.GetAxis("Horizontal");
        m_vertical = Input.GetAxis("Vertical");
        
        m_movementManager.playerController.gravity = 0;

        Vector3 ladderVelocity;
        if (!m_movementManager.controller.isGrounded)
        {
            ladderVelocity = new Vector3(m_horizontal, m_vertical, 0);
        }
        else
        {
            ladderVelocity = new Vector3(m_horizontal, m_vertical, m_vertical);
        }

        if (m_movementManager.playerCamera.IsLookingDown(ladderAngle))
            ladderVelocity.y = -m_vertical;

        float moveRate =  Mathf.Clamp(Mathf.Abs(m_movementManager.playerCamera.GetPitch()), 0, 2) * 2;
        ladderVelocity *= moveRate;
        Vector3 ladderDirection = transform.rotation * ladderVelocity;
        m_movementManager.controller.Move(ladderDirection * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_movementManager.isSwimming && other.CompareTag("Ladder"))
            m_movementManager.isClimbingLadder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_movementManager.isClimbingLadder)
            m_movementManager.isClimbingLadder = false;
    }
}
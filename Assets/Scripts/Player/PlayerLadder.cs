using UnityEngine;
using PlayerStates;

[RequireComponent(typeof(PlayerController))]
public class PlayerLadder : MonoBehaviour
{
    [Header("Properties")] 
    [Range(-90, 90)] public float ladderAngle = 15.0f;
    
    private CharacterController m_Controller;
    private PlayerStateManager m_MovementManager;
    private PlayerCamera m_pCamera;

    private void Start()
    {
        m_Controller      = GetComponent<CharacterController>();
        m_MovementManager = GetComponent<PlayerStateManager>();
        m_pCamera         = transform.GetChild(0).GetComponent<PlayerCamera>();
    }

    public void LadderVelocity()
    {
        m_MovementManager.playerController.gravity = 0;

        Vector3 ladderVelocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Vertical"));
        
        if (!m_Controller.isGrounded)
            ladderVelocity.z = 0;

        if (m_pCamera.pitch >= ladderAngle)
            ladderVelocity.y = -Input.GetAxis("Vertical");

        ladderVelocity *= Mathf.Clamp(Mathf.Abs(m_pCamera.pitch), 0, 2) * 2;
        m_Controller.Move(transform.rotation * ladderVelocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_MovementManager.currentState != PlayerState.Swimming && other.CompareTag("Ladder"))
            m_MovementManager.currentState = PlayerState.Ladder;
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_MovementManager.currentState == PlayerState.Ladder)
            m_MovementManager.currentState = PlayerState.Walking;
    }
}
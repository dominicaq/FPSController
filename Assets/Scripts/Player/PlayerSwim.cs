using UnityEngine;
using PlayerStates;

[RequireComponent(typeof(PlayerController))]
public class PlayerSwim : MonoBehaviour
{
    [Header("Properties")]
    public float swimMovementSpeed = 7.0f;
    public float ascendRate = 10;
    public float descendRate = 5;
    private Vector3 m_SwimVelocity;
    
    [Header("Components")]
    private PlayerStateManager m_MovementManager;
    private CharacterController m_Controller;

    private void Start()
    {
        m_MovementManager = GetComponent<PlayerStateManager>();
        m_Controller      = GetComponent<CharacterController>();
    }
    
    public void SwimVelocity()
    {
        float buoyancy = m_MovementManager.playerController.gravity;

        if (Input.GetButton("Jump"))
            buoyancy += ascendRate * Time.deltaTime;

        m_SwimVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        if (m_SwimVelocity.sqrMagnitude > 1)
            m_SwimVelocity = m_SwimVelocity.normalized;

        if (!m_Controller.isGrounded)
            buoyancy -= descendRate * Time.deltaTime;
        else
            buoyancy = 0;

        // Finalization
        Mathf.Clamp(buoyancy, -1, 3);
        m_MovementManager.playerController.gravity = buoyancy;
        m_SwimVelocity = transform.rotation * m_SwimVelocity * swimMovementSpeed;
        m_SwimVelocity.y = buoyancy;
        m_Controller.Move(m_SwimVelocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
            m_MovementManager.currentState = PlayerState.Swimming;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water") && m_MovementManager.currentState == PlayerState.Swimming)
            m_MovementManager.currentState = PlayerState.Walking;
    }
}
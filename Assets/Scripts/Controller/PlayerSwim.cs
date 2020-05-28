using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerSwim : MonoBehaviour
{
    [Header("Properties")]
    public float swimMovementSpeed = 7.0f;
    public float ascendRate = 10;
    public float descendRate = 5;
    private Vector3 m_swimVelocity;

    [Header("Input")] 
    private float m_horizontal;
    private float m_vertical;
    
    [Header("Components")] 
    private PlayerMovementManager m_movementManager;

    private void Start()
    {
        m_movementManager = GetComponent<PlayerMovementManager>();
    }
    
    /// <summary> Gives player control of their y axis position a trigger </summary>
    public void SwimVelocity()
    {
        m_horizontal = Input.GetAxis("Horizontal");
        m_vertical = Input.GetAxis("Vertical");
        
        m_swimVelocity = new Vector3(m_horizontal, 0, m_vertical);
        float buoyancy = m_movementManager.playerController.gravity;

        if (m_swimVelocity.sqrMagnitude > 1)
            m_swimVelocity = m_swimVelocity.normalized;

        if (Input.GetButton("Jump"))
        {
            buoyancy += ascendRate * Time.deltaTime;

            if (buoyancy >= 3)
                buoyancy = 3;
        }

        if (buoyancy < -1 && !m_movementManager.controller.isGrounded)
        {
            buoyancy += -buoyancy * Time.deltaTime;
        }
        else
        {
            buoyancy -= descendRate * Time.deltaTime;

            if (buoyancy <= -1)
                buoyancy = -1;
        }

        // Finalization
        m_movementManager.playerController.gravity = buoyancy;
        m_swimVelocity = transform.rotation * m_swimVelocity;
        m_swimVelocity *= swimMovementSpeed;
        m_swimVelocity.y = buoyancy;
        m_movementManager.controller.Move(m_swimVelocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
            m_movementManager.isSwimming = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water") && m_movementManager.isSwimming)
            m_movementManager.isSwimming = false;
    }
}
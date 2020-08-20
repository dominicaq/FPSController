using UnityEngine;

public class PlayerAntiStuck : MonoBehaviour
{
    private Vector3 m_PreviousPosition;
    private float m_pitch = 0;
    private float m_yaw = 0;
    private bool m_Teleport = false;

    private CharacterController m_Controller;
    private PlayerCamera m_pCamera;
    
    void Start()
    {
        m_Controller    = GetComponent<CharacterController>();
        m_pCamera       = transform.GetChild(0).GetComponent<PlayerCamera>();
    }

    void FixedUpdate()
    {
        if(m_Controller.isGrounded)
        {
            m_PreviousPosition = transform.position;
            m_pitch = m_pCamera.pitch;
            m_yaw = m_pCamera.yaw;
        }
            
        if(m_Teleport)
        {
            transform.position = m_PreviousPosition;
            m_pCamera.pitch = m_pitch;
            m_pCamera.yaw = m_yaw;

            m_Teleport = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "OutOfBounds" && !m_Controller.isGrounded)
            m_Teleport = true;
    }
}

using System.Collections;
using UnityEngine;
using PlayerStates;

public class PlayerCharge : MonoBehaviour
{
    public float duration= 1.5f;
    public float coolDown = 5.0f;
    public float speed = 15.0f;
    public float turnRate = 0.3f;
    public bool isCharging = false;
    private float m_CurrentDuration = 0.0f;
    private bool m_CanCharge = true;
    private WaitForSeconds chargeCD = new WaitForSeconds(5.0f);

    #region Components
    private PlayerController m_PlayerController;
    private PlayerCamera m_pCamera;

    #endregion

    void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
        m_pCamera          = transform.GetChild(0).GetComponent<PlayerCamera>();
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && m_CanCharge)
        {
            m_CanCharge = false;
            isCharging = true;
            m_PlayerController.enableCrouching = false;
        }
            
        if (isCharging)
        {
            m_CurrentDuration += Time.deltaTime;
            Charge();
        }
        
        if (m_CurrentDuration >= duration|| FrontCollision() || m_PlayerController.isSliding)
        {
            if(m_PlayerController.characterController.isGrounded)
            {
                isCharging = false;
                m_PlayerController.enableCrouching = true;
                
                m_pCamera.turnRate = m_pCamera.mouseSensitivity;
                m_CurrentDuration = 0;
            }

            StartCoroutine(ChargeCoolDown());
        }
    }

    private void Charge()
    {
        m_pCamera.turnRate = turnRate;
        Vector3 chargeVelocity = transform.forward + Vector3.down * .1f;
        m_PlayerController.characterController.Move(chargeVelocity * Time.deltaTime * speed);
    }

    private IEnumerator ChargeCoolDown()
    {
        yield return chargeCD;
        m_CanCharge = true;
    }

    private bool FrontCollision()
    {
        return Physics.Raycast(transform.position, transform.forward, 1);
    }
}

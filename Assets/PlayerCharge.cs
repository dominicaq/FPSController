using System.Collections;
using UnityEngine;

public class PlayerCharge : MonoBehaviour
{
    public float chargeCooldown = 5.0f;
    public float chargeSpeed = 10;
    public float chargeTime = 1.5f;
    private float m_chargeDuration = 0.0f;
    private bool m_canCharge = true;
    private PlayerMovementManager m_movementManager;

    void Start()
    {
        m_movementManager = GetComponent<PlayerMovementManager>();
    }

    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftShift) && m_canCharge && !m_movementManager.isSwimming)
        {
            m_movementManager.isCharging = true;
        }
        
        if(m_movementManager.isCharging)
        {
            m_chargeDuration += Time.deltaTime;
            Charge();
        }

        if((FrontCollision() && m_movementManager.isCharging) || m_chargeDuration >= chargeTime 
        || m_movementManager.isSwimming || m_movementManager.isSliding)
        {
            m_movementManager.isCharging = false;
            m_chargeDuration = 0;

            StartCoroutine(ChargeCoolDown());
        }
    }

    void Charge()
    {
        Vector3 chargeVelocity = transform.forward;

        m_movementManager.isCharging = true;
        m_movementManager.controller.Move(chargeVelocity * Time.deltaTime * chargeSpeed);
    }

    IEnumerator ChargeCoolDown()
    {
        m_canCharge = false;
        yield return new WaitForSeconds(chargeCooldown);
        m_canCharge = true;
    }

    private bool FrontCollision()
    {
        return Physics.Raycast(transform.position, transform.forward, 1);
    }
}

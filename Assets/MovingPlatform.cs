using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour, IListener
{
    #region Properties
    public float speed;
    public float pauseTime = 2.5f;
    public Transform platform;
    public Transform[] movePoints;
    private Rigidbody m_rb;
    private bool m_isActive = true;

    #endregion

    #region Platform Movement
    private CharacterController m_Controller;
    private WaitForSeconds m_Timer;    
    private int m_CurrentIndex = 0;
    private bool m_ResumePlatform = true;

    #endregion
    
    private void Start() 
    {
        m_Timer = new WaitForSeconds(pauseTime);
        m_rb    = platform.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(!m_isActive || !m_ResumePlatform)
            return;
        
        if(Vector3.Distance(platform.position, movePoints[m_CurrentIndex].position) >= 0.1f)
        {
            // Lerp because MoveTowards is too fast for rigidbodies
            float step = (Time.deltaTime * speed) / 100;
            Vector3 lerpedPos = Vector3.Lerp(platform.position, movePoints[m_CurrentIndex].position, step);
            m_rb.MovePosition(lerpedPos);
            
            if(m_Controller)
            {
                Vector3 currentDir = movePoints[m_CurrentIndex].position - platform.position;
                m_Controller.Move(currentDir * step);
            }      
        }
        else
        {
            StartCoroutine(FreezePlatform());
            if(m_CurrentIndex < movePoints.Length)
            {
                if (m_CurrentIndex == movePoints.Length-1)
                    m_CurrentIndex = 0;
                else
                    m_CurrentIndex++;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == 8)
        {
            m_Controller = other.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.layer == 8)
        {
            m_Controller = null;
        }
    }

    public void OnActivate()
    {
        m_isActive = true;
    }

    public void OnDeactivate()
    {
        m_isActive = false;
    }

    private IEnumerator FreezePlatform()
    {
        m_ResumePlatform = false;
        yield return m_Timer;
        m_ResumePlatform = true;
    }
}

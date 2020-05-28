using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoints : MonoBehaviour
{
    public Vector3[] currentPath;
    private int m_pointIndex = 1;
    private bool m_collided = false;
    private Rigidbody m_rb;
    private float m_initVelocity;

    public void LoadPointArray(Vector3[] newPath)
    {
        if(newPath.Length > 0)
        {
            currentPath = new Vector3[newPath.Length];
            currentPath = newPath;
            m_pointIndex = 0;

            m_rb = transform.GetComponent<Rigidbody>();
        }
    }
    
    public void SetInitVelocity(float velocity)
    {
        m_initVelocity = velocity;
    }

    void Update()
    {
        if(!m_collided && m_pointIndex != currentPath.Length)
        {
            float dist         = Vector3.Distance(transform.position, currentPath[m_pointIndex]);
            transform.position = Vector3.MoveTowards(transform.position, currentPath[m_pointIndex], Time.deltaTime * m_initVelocity);

            if(m_rb)
            {
                m_rb.angularVelocity = Vector3.zero;
                m_rb.velocity = Vector3.zero;
                m_rb.useGravity = false;
            }
                

            if(dist < 0.01f)
                m_pointIndex++;
        }
        else
        {
            if(m_rb)
                m_rb.useGravity = true;

            Destroy(this);
        }
    }

    private void OnCollisionStay(Collision other) 
    {
        m_collided = true;
    }
}

using UnityEngine;

public class EnergySpawner : MonoBehaviour, IListener
{
    public bool isActive = true;
    public GameObject energyBall;
    public float respawnDelay = 1.0f;
    private float m_CurrentTime = 0.0f;
    private GameObject m_CurrentObject;

    private void Update() 
    {
        if(!m_CurrentObject && isActive)
        {
            m_CurrentTime += Time.deltaTime;

            if(m_CurrentTime >= respawnDelay)
                m_CurrentObject = Instantiate(energyBall, transform.position + transform.forward, transform.rotation);
        }
        else
            m_CurrentTime = 0.0f;
    }

    public void OnActivate()
    {
        isActive = true;
    }

    public void OnDeactivate()
    {
        isActive = false;
    }
}

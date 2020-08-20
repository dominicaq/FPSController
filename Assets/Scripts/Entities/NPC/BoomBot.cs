using UnityEngine;

public class BoomBot : MonoBehaviour
{
    private DamageExplosion m_EntityExplosion;
    private bool m_StartClock = false;
    
    void Start()
    {
        m_EntityExplosion = GetComponent<DamageExplosion>();
    }

    public void OnDeath()
    {
        m_StartClock = true;
    }

    void Update()
    {
        if(m_StartClock)
            m_EntityExplosion.RunTimer();
    }
}

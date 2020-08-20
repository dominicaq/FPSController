using UnityEngine;
using TMPro;

public class StopWatchListener : MonoBehaviour, IListener
{
    public bool inverted = false;
    public GameObject[] listeners;
    private IListener[] m_Listeners;
    public float countdownTime = 5.0f;
    public float currentTime = 0.0f;
    public TextMeshProUGUI countdownText;
    private bool m_StartTimer = false;

    private void Start() 
    {
        currentTime = countdownTime;

        if(countdownText)
            countdownText.SetText(GetCountdownFormat());

        m_Listeners = new IListener[listeners.Length];
        for(int i = 0; i < listeners.Length; i++)
        {
            m_Listeners[i] = listeners[i].GetComponent<IListener>();
        }
    }

    private void Update()
    {
        if(!m_StartTimer)
            return;
        
        if(countdownText)
            countdownText.SetText(GetCountdownFormat());
        
        currentTime -= Time.deltaTime;
        if(currentTime <= 0)
        {
            currentTime = countdownTime;

            if(inverted)
                OnActivate();
            else
                OnDeactivate();
        }
    }

    public string GetCountdownFormat()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
        return string.Format("{00:00}:{1:00}", minutes, seconds);
    }

    public void OnActivate()
    {
        if(currentTime != countdownTime)
            return;

        for(int i = 0; i < listeners.Length; i++)
        {
            if(m_Listeners[i] != null)
                m_Listeners[i].OnActivate();
        }
        m_StartTimer = !inverted;
    }

    public void OnDeactivate()
    {
        if(currentTime != countdownTime)
            return;

        for(int i = 0; i < listeners.Length; i++)
        {
            if(m_Listeners[i] != null)
                m_Listeners[i].OnDeactivate();
        }
        m_StartTimer = inverted;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for(int i = 0; i < listeners.Length; i++)
        {
            if(listeners[i] != null)
                Gizmos.DrawLine(transform.position, listeners[i].transform.position);
        }
    }
}

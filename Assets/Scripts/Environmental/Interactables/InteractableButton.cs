using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public bool OneShot = false;
    private bool m_Alternate = true;
    private bool m_OneShot = false;
    public GameObject[] listeners;
    private IListener[] m_Listeners;
    private UnityEvent onListen;

    private void Start() 
    {
        m_Listeners = new IListener[listeners.Length];
        for(int i = 0; i < listeners.Length; i++)
        {
            m_Listeners[i] = listeners[i].GetComponent<IListener>();

            if(m_Listeners[i] == null)
                Debug.LogWarning("Gameobject does not have IListener: " + listeners[i].name );
        }
    }
    public void OnInteract()
    {
        if(m_OneShot)
            return;
        
        if(m_Alternate)
        {
            for(int i = 0; i < m_Listeners.Length; i++)
                m_Listeners[i].OnActivate();
            
            m_Alternate = false;
        }
        else
        {
            for(int i = 0; i < m_Listeners.Length; i++)
                m_Listeners[i].OnDeactivate();

            m_Alternate = true;
        }

        if(OneShot)
            m_OneShot = true;
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
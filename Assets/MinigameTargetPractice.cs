using UnityEngine;

public class MinigameTargetPractice : MonoBehaviour, IListener
{
    public int points;
    public GameObject[] targets;
    private IListener[] m_Listener;

    void Start()
    {
        for(int i = 0; i < targets.Length; i++)
        {            
            m_Listener[i] = targets[i].GetComponent<IListener>();
        }
    }

    void Update()
    {
        
    }

    public void OnActivate()
    {

    }

    public void OnDeactivate()
    {

    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        for(int i = 0; i < targets.Length; i++)
        {
            Gizmos.DrawLine(transform.position, targets[i].transform.position);
        }
    }
}

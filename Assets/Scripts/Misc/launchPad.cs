using UnityEngine;

public class launchPad : MonoBehaviour
{
    [Header("Graph")]
    public Transform start;
    public Transform target;
    [Range(0, 50)] public int resolution = 10;
    public float gravity = Physics.gravity.magnitude;
    public float velocity = 7.0f;
    public float angle = 45.0f;
    private Vector3[] m_trajectoryPoints;
    public float speed;
    
    [Header("Debug")]
    public bool showGraph;

    /// <summary> Update Y-Axis to an arc, Time must be 0-1 </summary>
    /// <param name="time"></param>
    private float CalculateTrajetory(float time)
    {
        // Wikipedia Trajetory calculation
        float radianAngle = angle * Mathf.Deg2Rad;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;
        
        float x  = time * maxDistance;   
        float vh = velocity * Mathf.Cos(radianAngle);

        speed = (maxDistance * Mathf.Sqrt(gravity) * Mathf.Sqrt(1 / Mathf.Cos(radianAngle))) / Mathf.Sqrt(2 * maxDistance * Mathf.Sin(radianAngle) + 2 * Mathf.Cos(radianAngle));
        return x * Mathf.Tan(radianAngle) - gravity * (x / vh) * (x / vh) / 2;
    }

    private Vector3[] UpdatePath()
    {
        Vector3[] ret = new Vector3[resolution];
        for(int i = 0; i < resolution; i++)
        {
            float t = i / (resolution - 1f);
            ret[i]    = Vector3.Lerp(start.position, target.position, t);
            ret[i].y -= CalculateTrajetory(t);
        }

        return ret;
    }

    private void Update() 
    {
        m_trajectoryPoints = UpdatePath();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!other.GetComponent<FollowPoints>())
        {
            FollowPoints follow = other.gameObject.AddComponent<FollowPoints>();
            follow.SendMessage("LoadPointArray", m_trajectoryPoints);
            follow.SendMessage("SetInitVelocity", velocity);
        }
        else
        {
            FollowPoints follow = other.GetComponent<FollowPoints>();
            follow.SendMessage("LoadPointArray", m_trajectoryPoints);
            follow.SendMessage("SetInitVelocity", velocity);
        }
    }
    
    private void OnDrawGizmos() 
    {
        if(showGraph && start && target)
        {
            Gizmos.color = Color.yellow;
            m_trajectoryPoints = UpdatePath();
            
            for(int i = 0; i < m_trajectoryPoints.Length; i++)
            {
                Gizmos.DrawSphere(m_trajectoryPoints[i], 0.25f);
            }
        }
    }
}

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Sentry : MonoBehaviour
{
    [Header("Sentry Properties")]
    public bool sentryEnabled = true;
    private Collider[] m_proximityAr = new Collider[2];
    private Collider[] m_emptyArr = new Collider[2];
    private Transform m_currentTarget;
    private Transform m_sentryHead;

    [Header("Seek Parameters")]
    [Range(0,50)] public float seekRange = 10.0f;
    [Range(0, 180)] public float seekAngle = 55.0f;
    public float baseSeekRate = 50.0f;
    
    [Header("Shooting")]
    public float fireRate = 1.0f;
    public float fireDelay = 1.0f;

    private void Start() 
    {
        m_sentryHead = transform.GetChild(0);
    }

    private void Update()
    {
        if(sentryEnabled)
        {
            int proximityCount = Physics.OverlapSphereNonAlloc(transform.position, seekRange, m_proximityAr, 1 << 8);
            m_currentTarget = FindTarget(proximityCount);

            // Seek player
            if(m_currentTarget)
            {
                float lockRate = 1;
                Quaternion targetRotation = Quaternion.LookRotation(m_currentTarget.position - transform.position);
                
                if(m_sentryHead.rotation == targetRotation)
                {
                    Fire();
                    lockRate = 5;
                }

                MoveHead(targetRotation, lockRate);
            }
            else
            {
                m_proximityAr = m_emptyArr;
                MoveHead(transform.rotation, 1);
            }
        }
    }

    private void Fire()
    {
        if(Physics.Raycast(m_sentryHead.transform.position, m_sentryHead.transform.forward, out RaycastHit hit, seekRange))
        {
            //Debug.Log("Fire");
        }
    }

    private void MoveHead(Quaternion targetRotation, float seekRate)
    {
        float step = Time.deltaTime * baseSeekRate * seekRate;
        m_sentryHead.rotation = Quaternion.RotateTowards(m_sentryHead.rotation, targetRotation, step);
    }

    // Find closest target to turret
    private Transform FindTarget(int targetsFound)
    {
        Transform closest = null;

        for(int i = 0; i < targetsFound; i++)
        {
            Vector3 resultPosition = m_proximityAr[i].transform.position;
            if (Physics.Linecast(transform.position, resultPosition, out RaycastHit hit))
            {
                if(hit.transform.gameObject.layer == 8)
                {
                    // Determine objects angle
                    Vector3 tempTargetDir = resultPosition - transform.position;
                    float incomingAngle = Vector3.Angle(tempTargetDir, transform.forward);

                    // If object is within bounds
                    if (incomingAngle <= seekAngle)
                    {
                        float currentDist = Vector3.Distance(resultPosition, transform.position);
                        closest = m_proximityAr[i].transform;                    
                    }
                }
            }
        }

        return closest;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() 
    {
        Color gizColor = Color.white;
        gizColor.a = 0.40f;

        // FOV Arc
        Handles.color = gizColor;
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, seekAngle, seekRange);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -seekAngle, seekRange);
    }
#endif
}

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Sentry : MonoBehaviour
{
    [Header("Sentry Properties")]
    public bool sentryEnabled = true;
    private Collider[] proximityArr = new Collider[2];
    private Transform currentTarget = null;
    private Transform sentryHead;
    private Quaternion initRotation;

    [Header("Seek Parameters")]
    [SerializeField] [Range(0,50)] private float seekRange = 10.0f;
    [SerializeField] [Range(0, 180)] private float seekAngle = 55.0f;
    [SerializeField] private float seekRate = 50.0f;
    private float seekAmp = 1.0f;
    
    [Header("Shooting")]
    public float fireRate = 1.0f;
    public float fireDelay = 1.0f;

    private void Start() 
    {
        sentryHead = transform.GetChild(0);
    }

    private void Update()
    {
        if(sentryEnabled)
        {
            int layerMask = 1 << 8;
            int proximityCount = Physics.OverlapSphereNonAlloc(transform.position, seekRange, proximityArr, layerMask);
            currentTarget = FindTarget(proximityCount);

            if(currentTarget)
            {
                // Seeking player
                Quaternion targetDir = Quaternion.LookRotation(currentTarget.position - transform.position);
                MoveHead(targetDir);
            
                if(sentryHead.rotation == targetDir)
                {
                    Fire();
                    seekAmp = 5;
                }
            }
            else
            {
                proximityArr = new Collider[2];
                seekAmp = 1;

                initRotation = transform.rotation;
                MoveHead(initRotation);
            }
        }
    }

    private void Fire()
    {
        if(Physics.Raycast(sentryHead.transform.position, sentryHead.transform.forward, out RaycastHit hit, seekRange))
        {
            //Debug.Log("Fire");
        }
    }

    private void MoveHead(Quaternion desiredRotation)
    {
        float step = Time.deltaTime * seekRate * seekAmp;
        sentryHead.rotation = Quaternion.RotateTowards(sentryHead.rotation, desiredRotation, step);
    }

    // Find closest target to turret
    private Transform FindTarget(int targetsFound)
    {
        Transform closest = null;
        float distance = Mathf.Infinity;

        for(int i = 0; i < targetsFound; i++)
        {
            Transform resultObj = proximityArr[i].transform;

            // Line of Sight
            if (Physics.Linecast(transform.position, resultObj.position, out RaycastHit hit))
            {
                if(hit.transform.gameObject.layer == 8)
                {
                    // Determine objects angle
                    Vector3 tempTargetDir = resultObj.position - transform.position;
                    tempTargetDir.y = 0;

                    float incomingAngle = Vector3.Angle(tempTargetDir, transform.forward);

                    // If object is within bounds
                    if (incomingAngle <= seekAngle)
                    {
                        float currentDist = Vector3.Distance(resultObj.position, transform.position);

                        if (currentDist < distance)
                        {
                            closest = resultObj;
                            distance = currentDist;
                        }                        
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
        gizColor.a = 0.42f;

        // FOV Arc
        Handles.color = gizColor;
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, seekAngle, seekRange);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -seekAngle, seekRange);
    }
#endif
}

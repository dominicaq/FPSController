using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Sentry : EntityHealth
{
    [System.NonSerialized] public bool sentryEnabled = true;

    [Header("Sentry Properties")]
    [Range(0,50)] public float seekRange = 10.0f;
    [Range(0, 180)] public float seekAngle = 55.0f;
    public float baseSeekRate = 50.0f;

    [Header("Shooting")]
    public float fireDamage = 2.0f;
    public float fireRate = 0.25f;
    private EntityHealth m_EntityHP;
    private PlayerForce m_ForceModifier;
    private bool m_CanFire = true;

    #region Game info

    private Collider[] m_ProximityArr = new Collider[2];
    private Collider[] m_EmptyArr = new Collider[2];
    private Transform m_CurrentTarget;
    private Transform m_SentryHead;

    #endregion

    private void Start() 
    {
        m_SentryHead = transform.GetChild(0);
    }

    private void Update()
    {
        if(!sentryEnabled)
            return;

        float lockRate = 1;
        int proximityCount = Physics.OverlapSphereNonAlloc(transform.position, seekRange, m_ProximityArr, 1 << 8);

        // Seek player
        if(FindTarget(proximityCount))
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_CurrentTarget.position - transform.position);
            
            if(m_SentryHead.rotation == targetRotation)
            {
                Fire();
                lockRate = 5;
            }

            MoveHead(targetRotation, lockRate);
        }
        else
        {
            m_ProximityArr = m_EmptyArr;
            MoveHead(transform.rotation, lockRate);
        }
    }

    private void Fire()
    {
        if(Physics.Raycast(m_SentryHead.transform.position, m_SentryHead.transform.forward, out RaycastHit hit, seekRange) && m_CanFire)
        {
            StartCoroutine(DamageInterval());
            m_EntityHP.SendDamage(fireDamage);
            m_ForceModifier.AddForce(-m_SentryHead.forward * (fireDamage + 1));
        }
    }

    private IEnumerator DamageInterval()
    {
        m_CanFire = false;
        yield return new WaitForSeconds(fireRate);
        m_CanFire = true;
    }

    private void MoveHead(Quaternion targetRotation, float seekRate)
    {
        float step = Time.deltaTime * baseSeekRate * seekRate;
        m_SentryHead.rotation = Quaternion.RotateTowards(m_SentryHead.rotation, targetRotation, step);
    }

    // Find closest target to turret
    private bool FindTarget(int targetsFound)
    {
        for(int i = 0; i < targetsFound; i++)
        {
            Vector3 resultPosition = m_ProximityArr[i].transform.position;
            if (Physics.Linecast(transform.position, resultPosition, out RaycastHit hit))
            {
                if(hit.collider.CompareTag("Unit"))
                {
                    // Determine objects angle
                    Vector3 tempTargetDir = resultPosition - transform.position;
                    float incomingAngle   = Vector3.Angle(tempTargetDir, transform.forward);

                    // If object is within bounds
                    if (incomingAngle <= seekAngle)
                    {
                        m_CurrentTarget   = m_ProximityArr[i].transform;
                        m_EntityHP        = m_CurrentTarget.GetComponent<EntityHealth>();
                        m_ForceModifier   = m_CurrentTarget.GetComponent<PlayerForce>();
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void OnDeath()
    {
        sentryEnabled = false;
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

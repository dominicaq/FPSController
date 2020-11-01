using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [Header("Properties")]
    public float useDistance = 2.0f;
    public float maxDistance = 7.0f;
    [Range(1, 15)] public float moveRate = 15;
    public float floorDir = -0.45f;
    public float liftCapacity = 2;
    public float throwStrength = 3;

    #region Conditions
    public bool isCarrying;
    private bool canDrop = true;

    #endregion
    
    #region Held Object
    private Transform m_HeldObject;
    private Rigidbody m_HeldObjectRb;
    private Collider m_HeldObjectCollider;
    private LayerMask m_HeldObjectMask;

    #endregion

    #region Player
    public LayerMask ignoreLayer;
    private Protagonist protagonist;
    private CameraController cameraController;
    private Collider playerCollider;
    private Transform cameraTransform;


    #endregion

    // TODO: turn this script into a ability (Make a ability manager script)
    private void Start()
    {
        protagonist      = GetComponent<Protagonist>();
        cameraTransform  = protagonist.cameraTransform;
        cameraController = protagonist.cameraController;
        playerCollider   = GetComponent<Collider>();
    }

    public void Use()
    {
        if (canDrop)
        {
            if (!isCarrying)
                PickUp();
            else
                Drop();
        }
    }

    public void Fire()
    {
        ThrowObject(m_HeldObjectRb);
    }

    private void Update()
    {
        if(!m_HeldObject)
            return;

        Vector3 dist = m_HeldObject.position - transform.position;
        if (dist.sqrMagnitude > maxDistance){
            Drop();
            return;
        }

        canDrop = dist.sqrMagnitude >= 1.25f;
        Physics.IgnoreCollision(m_HeldObjectCollider, playerCollider, !canDrop);

        Quaternion desiredRot = transform.rotation;
        if (Physics.SphereCast(cameraTransform.position, 0.5f, SpringPosition() - cameraTransform.position, out RaycastHit hit, useDistance, ~ignoreLayer, QueryTriggerInteraction.Ignore)){
            desiredRot = Quaternion.FromToRotation(-m_HeldObject.forward, hit.normal) * m_HeldObject.rotation;
        }
            
        m_HeldObject.position = Vector3.Lerp(m_HeldObject.position, SpringPosition(), Time.deltaTime * moveRate);
        m_HeldObject.rotation = Quaternion.RotateTowards(m_HeldObject.rotation, desiredRot, Time.deltaTime * 250);
    }

    private void ThrowObject(Rigidbody body)
    {
        if(body)
        {
            Drop();
            body.AddForce(cameraTransform.forward * throwStrength);
        }
    }

    private Vector3 GetPrefferedDirection()
    {
        Vector3 prefferedDir = cameraTransform.rotation * Vector3.forward;

        if (Physics.SphereCast(m_HeldObject.position, 0.20f, Vector3.down, out RaycastHit hit, 0.75f, ~ignoreLayer, QueryTriggerInteraction.Ignore) && cameraController.pitch > -10f){
            prefferedDir = transform.rotation * Vector3.forward;
            prefferedDir = prefferedDir.normalized * (hit.distance + 0.25f);
        }
        else if (cameraController.pitch >= 20){
            prefferedDir = transform.rotation * Vector3.forward;
            prefferedDir.y = floorDir;
        }

        return prefferedDir.normalized;
    }

    private Vector3 SpringPosition(float objectRadius = 0.20f)
    {
        Vector3 prefferedVector = GetPrefferedDirection();
        if (Physics.SphereCast(cameraTransform.position, objectRadius, prefferedVector, out RaycastHit hit, useDistance, ~ignoreLayer, QueryTriggerInteraction.Ignore))
            return prefferedVector * (hit.distance - objectRadius * 2.0f) + cameraTransform.position;

        return prefferedVector * 1.75f + cameraTransform.position;
    }

    private void PickUp()
    {
        if (Physics.Raycast(cameraController.centerOfScreenRay, out RaycastHit hit, useDistance, ~LayerMask.GetMask("Interactive")))
        {
            if (hit.rigidbody && hit.rigidbody.mass <= liftCapacity)
            {
                m_HeldObject                  = hit.transform;
                m_HeldObjectMask              = m_HeldObject.gameObject.layer;
                m_HeldObject.gameObject.layer = 28;
                m_HeldObjectCollider          = m_HeldObject.GetComponent<Collider>();

                m_HeldObjectRb                = hit.rigidbody;
                m_HeldObjectRb.isKinematic    = true;
                
                isCarrying = true;
                cameraController.ShowViewModel(false);
            }
        }
    }

    private void Drop()
    {
        m_HeldObject.gameObject.layer = m_HeldObjectMask;
        m_HeldObjectRb.isKinematic    = false;
        
        m_HeldObject         = null;
        m_HeldObjectCollider = null;
        m_HeldObjectRb       = null;
        
        isCarrying = false;
        cameraController.ShowViewModel(true);
    }

    private void OnDrawGizmos()
    {
        if(!isCarrying)
            return;
        
        Gizmos.color = new Color(0, 1, 0.5f, 0.5f);
        Vector3 springDebug = SpringPosition();
        Gizmos.DrawLine(cameraTransform.position, springDebug);
        Gizmos.DrawSphere(springDebug, 1);
    }
}
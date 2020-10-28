using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [Header("Properties")]
    public float useDistance = 2.0f;
    [Range(1, 15)] public float moveRate = 15;
    public float liftCapacity = 2;
    public float throwStrength = 3;

    #region Conditions
    private bool m_IsCarrying;
    private bool m_IsColliding;
    private bool m_CanDrop = true;

    #endregion
    
    #region Held Object
    private Transform m_HeldObject;
    private Rigidbody m_HeldObjectRb;

    // Collisions
    private Collider m_HeldObjectCollider;
    private Bounds   m_HeldObjectBounds;

    #endregion

    #region Player

    private Collider m_PlayerColl;
    private Camera m_Camera;
    private CameraController m_CameraController;
    private Transform tCamera;
    private int m_IgnoreMask;

    #endregion

    // Test
    // TODO: turn this script into a ability

    private void Start()
    {
        tCamera = transform.GetChild(0).GetChild(0);
        m_CameraController = tCamera.GetComponent<CameraController>();
        m_Camera       = tCamera.GetComponent<Camera>();
        m_PlayerColl   = GetComponent<Collider>();
        
        m_IgnoreMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask("Player");
    }

    public void Use()
    {
        if (m_CanDrop)
        {
            if (!m_IsCarrying)
            {
                PickUp();
            }
            else
            {
                Drop();
            }
        }
    }

    public void Fire()
    {
        ThrowObject(m_HeldObjectRb);
    }

    private void Update()
    {
        if(m_IsCarrying)
        {
            m_HeldObjectBounds = m_HeldObjectCollider.bounds;
            m_IsColliding = Physics.CheckBox(m_HeldObject.position, m_HeldObjectBounds.size / 2, m_HeldObject.rotation, m_IgnoreMask, QueryTriggerInteraction.Ignore);
            MaintainDistance();

            // Fire was here

            // Movement for no collisions
            if (m_HeldObject && !m_IsColliding)
            {
                float step = Time.deltaTime * moveRate;
                m_HeldObject.position = Vector3.Lerp(m_HeldObject.position, GetPrefferedPosition(), step);
            
                Quaternion targetRot = transform.rotation;
                m_HeldObject.rotation = Quaternion.RotateTowards(m_HeldObject.rotation, targetRot, step * 25);
            }
        }
    }

    private void FixedUpdate()
    {
        // Movement for collisions
        if(m_IsCarrying && m_IsColliding)
        {
            NullifyRbVelocity(m_HeldObjectRb);
            m_HeldObject.position = Vector3.Lerp(m_HeldObject.position, GetPrefferedPosition(), (Time.smoothDeltaTime * moveRate) / 2.5f);
        }
    }

    private void ThrowObject(Rigidbody body)
    {
        if(body)
        {
            Drop();
            body.AddForce(transform.forward * throwStrength);
        }
    }
    
    private Vector3 GetPrefferedPosition()
    {
        Vector3 forwardVector = Vector3.forward * 1.75f;
        Vector3 viewEuler = tCamera.rotation * forwardVector;

        // If the player is looking down, clamp preffered position
        if (m_CameraController.pitch >= 25)
        {   
            viewEuler = transform.rotation * forwardVector;
            viewEuler.y = -.50f;
        }

        return tCamera.position + viewEuler;
    }

    private void MaintainDistance()
    {
        Vector3 dist = m_HeldObject.position - transform.position;
        if (dist.sqrMagnitude > useDistance + 5f)
        {
            Drop();
        }
        else if (dist.sqrMagnitude <= 1.0f)
        {
            Physics.IgnoreCollision(m_HeldObjectCollider, m_PlayerColl);
            m_CanDrop = false;
        }
        else
        {
            Physics.IgnoreCollision(m_HeldObjectCollider, m_PlayerColl, false);
            m_CanDrop = true;
        }
    }

    private void PickUp()
    {
        Ray interactRay = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(interactRay, out RaycastHit hit, useDistance, ~LayerMask.GetMask("Interactive")))
        {
            if (hit.rigidbody && hit.rigidbody.mass <= liftCapacity)
            {
                m_HeldObject                  = hit.transform;
                m_HeldObject.gameObject.layer = 2; // Ignore raycast layer (this is in general bad, dont know how to fix for box collider)
                m_HeldObjectCollider          = m_HeldObject.GetComponent<Collider>();

                m_HeldObjectRb                = hit.rigidbody;
                m_HeldObjectRb.interpolation  = RigidbodyInterpolation.Extrapolate;
                m_HeldObjectRb.useGravity     = false;
                
                m_IsCarrying = true;
                m_CameraController.SetViewModel(false);
            }
        }
    }

    private void Drop()
    {
        m_HeldObject.gameObject.layer = 0;
        m_HeldObjectRb.interpolation = RigidbodyInterpolation.None;
        m_HeldObjectRb.useGravity = true;
        
        m_HeldObject         = null;
        m_HeldObjectCollider = null;
        m_HeldObjectRb       = null;
        
        m_IsCarrying = false;
        m_CameraController.SetViewModel(true);
    }

    private void NullifyRbVelocity(Rigidbody body)
    {
        body.angularVelocity = Vector3.zero;
        body.velocity        = Vector3.zero;
    }
    
    private void OnDrawGizmos()
    {
        if(!m_IsCarrying)
            return;
        
        Gizmos.color = new Color(0, 1, 0.5f, 0.5f);
        Gizmos.DrawCube(m_HeldObject.position, m_HeldObjectBounds.size);
    }
}
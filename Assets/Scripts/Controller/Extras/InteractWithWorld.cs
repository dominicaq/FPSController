using UnityEngine;
public class InteractWithWorld : MonoBehaviour
{
    [Header("Interact properties")] private bool isCarrying;
    private bool isColliding;
    [SerializeField] private float throwStrength = 3;
    [SerializeField] private float interactArmLength = 2.0f;
    [SerializeField] private float liftCapacity = 2;
    [SerializeField] [Range(1, 15)] private float moveRate = 1;

    [Header("Held Object")] [SerializeField]
    private Transform heldObject;
    private Vector3 rotatedVector = Vector3.zero;
    private Vector3 holdVector;
    private Rigidbody heldObjectRb;

    [Header("Colliders")] private Collider heldObjectCollider;
    private Collider playerCollider;

    [Header("Player")] private Camera selfCam;
    private PlayerCamera playerCamera;
    private int ignorePlayerMask;

    private void Start()
    {
        playerCamera = transform.GetComponent<PlayerCamera>();
        selfCam = transform.GetComponent<Camera>();

        playerCollider = transform.parent.GetComponent<Collider>();
        ignorePlayerMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (!isCarrying)
            {
                PickUp();
            }
            else
            {
                Drop();
            }
        }

        if (Input.GetButton("Fire1") && isCarrying)
            ThrowRigid(heldObjectRb);
        
        // Movement type for no collisions present
        if (!isColliding && isCarrying)
        {
            float step = Time.deltaTime * moveRate;
            heldObject.position = Vector3.Lerp(heldObject.position, holdVector, step);
            
            Quaternion targetRot = transform.parent.rotation;
            heldObject.rotation = Quaternion.RotateTowards(heldObject.rotation, targetRot, step * 25);
        }
    }

    private void FixedUpdate()
    {
        // Hold the object
        if (heldObject && heldObjectRb)
        {
            isCarrying = true;
            
            // Holding
            Vector3 forwardVector = Vector3.forward * 1.75f;
            if (playerCamera.IsLookingDown(40))
            {
                forwardVector = Vector3.forward * 1.5f;
                rotatedVector = transform.parent.rotation * forwardVector;
                rotatedVector.y = -1f;
            }
            else
            {
                rotatedVector = transform.rotation * forwardVector;
            }
            
            NullifyRbForces();
            holdVector = transform.position + rotatedVector;

            Vector3 boxSize = heldObjectCollider.bounds.size;
            isColliding = Physics.CheckBox(heldObject.position, boxSize / 2, heldObject.rotation, ignorePlayerMask, QueryTriggerInteraction.Ignore);
            if (isColliding && isCarrying)
            {
                heldObject.position = Vector3.MoveTowards(heldObject.transform.position, holdVector,
                    Time.deltaTime * moveRate / 3);
            }

            // Enforce distance between object
            Vector3 offset = heldObject.position - transform.position;
            float dist = offset.sqrMagnitude;
            if (dist > interactArmLength + 1f)
            {
                Drop();
            }
        }
        else
        {
            heldObject = null;
            heldObjectRb = null;
        }
    }
    
    private void ThrowRigid(Rigidbody body)
    {
        if (body)
        {
            Drop();
            body.AddForce(transform.forward * throwStrength);
        }
    }
    
    private void PickUp()
    {
        Ray interactRay = selfCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(interactRay, out RaycastHit hit, interactArmLength))
        {
            heldObject = hit.transform;
            heldObjectRb = hit.rigidbody;
            heldObject.gameObject.layer = 2; // Ignore raycast layer

            if (heldObject && heldObjectRb)
            {
                if (hit.rigidbody.mass <= liftCapacity)
                {
                    // Ignore all collisions in object
                    heldObjectCollider = heldObject.GetComponent<Collider>();
                    Physics.IgnoreCollision(heldObjectCollider, playerCollider);
                    foreach (Transform child in heldObject)
                    {
                        Collider childCollider = child.GetComponent<Collider>();
                        if (!childCollider)
                            break;

                        child.gameObject.layer = LayerMask.NameToLayer("Player");
                        Physics.IgnoreCollision(childCollider, playerCollider);
                    }
                }
            }
        }
    }

    private void Drop()
    {
        Physics.IgnoreCollision(heldObjectCollider, playerCollider, false);
        foreach (Transform child in heldObject)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (!childCollider)
                break;

            child.gameObject.layer = LayerMask.NameToLayer("Default");
            Physics.IgnoreCollision(childCollider, playerCollider, false);
        }

        heldObject.gameObject.layer = 0;
        heldObjectRb.useGravity = true;

        heldObjectCollider = null;
        heldObjectRb = null;
        heldObject = null;

        isCarrying = false;
    }

    private void NullifyRbForces()
    {
        heldObjectRb.angularVelocity = Vector3.zero;
        heldObjectRb.velocity = Vector3.zero;
        heldObjectRb.useGravity = false;
    }
}
using UnityEngine;

public class GravGun : MonoBehaviour
{
    [Header("Gravity gun properties")]
    private bool isCarrying = false;
    private bool isColliding = false;
    private Transform reference;
    [SerializeField] private float interactArmLength = 2.0f;
    [SerializeField] private float liftCapacity = 5;
    [SerializeField] [Range(1, 15)] private float moveRate = 1;
    
    [Header("Held Object")]
    [SerializeField] private Transform heldObject;
    private Vector3 holdVector;
    private Vector3 rotatedVector = Vector3.zero;
    private Rigidbody heldObjectRB;
    private Collider[] hitColliders;

    [Header("Camera")]
    private Camera selfCam;
    private PlayerCamera playerCamera;

    private void Start()
    {
        reference = transform.parent.parent;
        playerCamera = reference.GetComponent<PlayerCamera>();
        selfCam = reference.GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            if(!isCarrying)
            {
                PickUp();
            }
            else
            {
                Drop();
            }
        }

        if(Input.GetButtonDown("Fire1") && isCarrying)
        {

        }
    }

    private void FixedUpdate() 
    {
        // Hold the object
        if(heldObject != null && heldObjectRB != null)
        {
            isCarrying = true;
            
            if(!playerCamera.IsLookingDown(65))
            {
                rotatedVector = reference.rotation * (Vector3.forward * 1.5f);
            }

            // Rotation
            Vector3 desiredRot     = new Vector3(0, reference.eulerAngles.y, reference.eulerAngles.z);
            heldObject.eulerAngles = desiredRot;

            holdVector = rotatedVector + reference.position;
            
            // Movement for colliding (Not efficent but works)
            hitColliders = Physics.OverlapSphere(heldObject.position, heldObject.localScale.x);
            if(hitColliders.Length > 1)
            {
                for(int i = 0; i < hitColliders.Length; i++)
                {
                    if(hitColliders[i].transform.name != heldObject.name && hitColliders[i].transform.gameObject.layer != 8)
                    {
                        isColliding = true;
                    }
                }
            }
            else
                isColliding = false;
            
            NullifyRbForces();

            // Enforce distance between object
            float dist = Vector3.Distance(heldObject.position, reference.position);
            if(dist > interactArmLength + .2f)
                Drop();

            if(isColliding && isCarrying)
                heldObject.position = Vector3.MoveTowards(heldObject.transform.position, holdVector, Time.deltaTime * moveRate/2);    
        }
    }

    private void LateUpdate()
    {
        // Movement type for no collisions present
        if(!isColliding && isCarrying)
            heldObject.position = Vector3.Lerp(heldObject.position, holdVector, Time.deltaTime * moveRate);
    }

    private void PickUp()
    {
        Ray interactRay = selfCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(interactRay, out RaycastHit hit, interactArmLength))
        {
            // Prevent grabbing sizes too big
            float sizeCheck = hit.transform.localScale.x + hit.transform.localScale.y + hit.transform.localScale.z;

            if(hit.rigidbody != null && sizeCheck <= liftCapacity)
            {
                // Go through all child objects and make their colliders ignore player
                Physics.IgnoreCollision(hit.transform.GetComponent<Collider>(), reference.parent.GetComponent<Collider>());
                foreach(Transform child in hit.transform)
                {
                    if(child.GetComponent<Collider>() == null)
                        break;
                    
                    Physics.IgnoreCollision(child.GetComponent<Collider>(), reference.parent.GetComponent<Collider>());
                }
                
                heldObjectRB = hit.rigidbody;
                heldObject   = hit.transform;
                heldObject.gameObject.layer = 2;
            }
        }
    }

    private void Drop()
    {
        Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), reference.parent.GetComponent<Collider>(), false);
        heldObjectRB.useGravity = true;
        heldObject.gameObject.layer = 0;
        isCarrying = false;

        heldObjectRB = null;
        heldObject = null;
    }

    private void NullifyRbForces()
    {
        heldObjectRB.angularVelocity = Vector3.zero;
        heldObjectRB.velocity        = Vector3.zero;
        heldObjectRB.useGravity      = false;
    }
}
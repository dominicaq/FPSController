using UnityEngine;

public class InteractWorld : MonoBehaviour
{
    [Header("Interact properties")]
    [SerializeField] private bool isCarrying = false;
    [SerializeField] private float interactArmLength = 2.0f;
    [SerializeField] private float liftCapacity = 5;
    [SerializeField] [Range(1, 15)] private float moveRate = 1;
    [SerializeField] private bool isColliding = false;
    
    [Header("Held Object")]
    [SerializeField] private Transform heldObject;
    private Vector3 holdVector;
    private Vector3 rotatedVector = Vector3.zero;
    private Rigidbody heldObjectRB;
    private Collider[] hitColliders;

    [Header("Camera")]
    private Camera selfCam;
    private PlayerCamera playerCamera;

    private void Awake()
    {
        playerCamera = transform.GetComponent<PlayerCamera>();
        selfCam = transform.GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Interact"))
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
    }

    private void FixedUpdate() 
    {
        // Hold the object
        if(heldObject != null && heldObjectRB != null)
        {
            isCarrying = true;
            
            if(!playerCamera.isLookingDown(65))
            {
                rotatedVector = rotatedVector = transform.rotation * (Vector3.forward * 1.5f);
            }

            // Rotation
            Vector3 desiredRot = transform.eulerAngles;
            desiredRot.x = 0;
            heldObject.eulerAngles = desiredRot;

            holdVector = rotatedVector + transform.position;
            
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
            
            float dist = Vector3.Distance(heldObject.position, transform.position);
            if(dist > interactArmLength + .2f)
                Drop();
        }

        if(isColliding && isCarrying)
            heldObject.position = Vector3.MoveTowards(heldObject.transform.position, holdVector, Time.deltaTime * moveRate/2);
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
                Physics.IgnoreCollision(hit.transform.GetComponent<Collider>(), transform.parent.GetComponent<Collider>());
                foreach(Transform child in hit.transform)
                {
                    if(child.GetComponent<Collider>() == null)
                        break;
                    
                    Physics.IgnoreCollision(child.GetComponent<Collider>(), transform.parent.GetComponent<Collider>());
                }
                
                heldObject = hit.transform;
                heldObject.gameObject.layer = 2;
                heldObjectRB = hit.rigidbody;
            }
        }
    }

    private void Drop()
    {
        Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), transform.parent.GetComponent<Collider>(), false);
        heldObjectRB.useGravity = true;
        heldObject.gameObject.layer = 0;
        isCarrying = false;

        heldObjectRB = null;
        heldObject = null;
    }

    private void NullifyRbForces()
    {
        heldObjectRB.velocity = Vector3.zero;
        heldObjectRB.useGravity = false;
        heldObjectRB.angularVelocity = Vector3.zero;
    }
}

using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class InteractWorld : MonoBehaviour
{
    [Header("Interact properties")]
    private bool isCarrying = false;
    private bool isColliding = false;
    [SerializeField] private float interactArmLength = 2.0f;
    [SerializeField] private float liftCapacity = 2;
    [SerializeField] [Range(1, 15)] private float moveRate = 1;

    [Header("Held Object")]
    [SerializeField] private Transform heldObject;
    private Vector3 rotatedVector = Vector3.zero;
    private Vector3 holdVector;
    private Rigidbody heldObjectRB;

    [Header("Colliders")]
    private Collider heldObjectCollider;
    private Collider playerCollider;
    
    [Header("Player")]
    private Camera selfCam;
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
        
        // Movement type for no collisions present
        if(!isColliding && isCarrying)
            heldObject.position = Vector3.Lerp(heldObject.position, holdVector, Time.deltaTime * moveRate);
    }

    private void FixedUpdate() 
    {
        // Hold the object
        if(heldObject && heldObjectRB)
        {
            isCarrying = true;
            Vector3 boxSize = heldObjectCollider.bounds.size;
            if (boxSize.y > 1)
            {
                boxSize.y += .87f;
            }
            
            // Holding
            Vector3 forwardVector = Vector3.forward * 1.75f;
            rotatedVector = transform.rotation * forwardVector;

            if (Physics.Raycast(heldObject.position, Vector3.down, boxSize.y) && rotatedVector.y <= -1.3f)
            {
                if (playerCamera.IsLookingDown((80)))
                {
                    float signX = Mathf.Sign(rotatedVector.x);
                    float signZ = Mathf.Sign(rotatedVector.z);
                    rotatedVector.x = Mathf.Clamp(rotatedVector.x + 1.5f, 1.3f * signX, Mathf.Infinity);
                    rotatedVector.z = Mathf.Clamp(rotatedVector.z + 1.5f, 1.3f * signZ, Mathf.Infinity);
                }
                rotatedVector.y = -1.3f;
            }
            
            holdVector = transform.position + rotatedVector;

            // Object Rotation
            Vector3 desiredRot = transform.eulerAngles;
            desiredRot.x = 0;
            heldObject.eulerAngles = desiredRot;
            
            NullifyRbForces();
            
            isColliding = Physics.CheckBox(heldObject.position, boxSize / 2, heldObject.rotation, ignorePlayerMask);
            if (isColliding && isCarrying)
            {
                heldObject.position = Vector3.MoveTowards(heldObject.transform.position, holdVector, Time.deltaTime * moveRate / 3);
            }
            
            // Enforce distance between object
            float dist = Vector3.Distance(heldObject.position, transform.position);
            if (dist > interactArmLength + 1f)
            {
                Drop();
            }
        }
    }

    private void PickUp()
    {
        Ray interactRay = selfCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(interactRay, out RaycastHit hit, interactArmLength))
        {
            // Prevent grabbing sizes too big
            Transform hitObject = hit.transform;
            float sizeCheck = hitObject.localScale.magnitude;

            if(hit.rigidbody && sizeCheck <= liftCapacity)
            {
                heldObjectCollider = hitObject.GetComponent<Collider>();
                Physics.IgnoreCollision(heldObjectCollider, playerCollider);
                foreach(Transform child in hitObject)
                {
                    Collider childCollider = child.GetComponent<Collider>();
                    if(!childCollider)
                        break;
                    
                    Physics.IgnoreCollision(childCollider, playerCollider);
                }
                
                heldObject = hit.transform;
                heldObjectRB = hit.rigidbody;
                heldObject.gameObject.layer = 2; // Ignore raycast layer
            }
        }
    }

    private void Drop()
    {
        Physics.IgnoreCollision(heldObjectCollider, playerCollider, false);
        heldObject.gameObject.layer = 0;
        heldObjectRB.useGravity = true;
        
        heldObjectCollider = null;
        heldObjectRB = null;
        heldObject = null;
        
        isCarrying = false;
    }

    private void NullifyRbForces()
    {
        heldObjectRB.angularVelocity = Vector3.zero;
        heldObjectRB.velocity        = Vector3.zero;
        heldObjectRB.useGravity      = false;
    }
}
﻿using UnityEngine;

public class InteractWithWorld : MonoBehaviour
{
    [Header("Interact properties")] private bool isCarrying;
    private bool isColliding;
    [SerializeField] private float throwStrength = 3;
    [SerializeField] private float interactArmLength = 2.0f;
    [SerializeField] private float liftCapacity = 2;
    [SerializeField] [Range(1, 15)] private float moveRate = 1;

    [Header("Held Object")]
    private Transform heldObject;
    private Vector3 rotatedVector = Vector3.zero;
    private Vector3 holdVector;
    private Rigidbody heldObjectRb;

    [Header("Collider")] 
    private Collider heldObjectCollider;
    private Bounds heldObjectBounds;
    private Collider playerCollider;
    private GameObject tempObject;

    [Header("Player")] private Camera selfCam;
    private PlayerCamera playerCamera;
    private int ignoreUnitMask;

    private void Start()
    {
        playerCamera = transform.GetComponent<PlayerCamera>();
        selfCam = transform.GetComponent<Camera>();

        playerCollider = transform.parent.GetComponent<Collider>();
        ignoreUnitMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask("IgnoreUnit");
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
            if (playerCamera.IsLookingDown(17))
            {
                forwardVector = Vector3.forward * 2f;
                rotatedVector = transform.parent.rotation * forwardVector;
                rotatedVector.y = -.35f; // Was 1
            }
            else
            {
                rotatedVector = transform.rotation * forwardVector;
            }
            
            NullifyRbForces();
            holdVector = transform.position + rotatedVector;
            
            heldObjectBounds = heldObjectCollider.bounds;
            Vector3 calculatedPos = heldObject.position;
            calculatedPos.y = heldObjectBounds.center.y;

            isColliding = Physics.CheckBox(calculatedPos, heldObjectBounds.size / 2, heldObject.rotation, ignoreUnitMask, QueryTriggerInteraction.Ignore);
            if (isColliding && isCarrying)
            {
                heldObject.position = Vector3.MoveTowards(heldObject.position, holdVector,
                    Time.smoothDeltaTime * moveRate / 4);
            }

            // Enforce distance between object
            Vector3 offset = heldObject.position - transform.position;
            float dist = offset.sqrMagnitude;
            if (dist > interactArmLength + 5f)
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
                    
                    heldObjectCollider = heldObject.GetComponent<Collider>();
    
                    // Ignore all collisions in object
                    foreach (Transform child in heldObject)
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("IgnoreUnit");
                    }
                    
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    private void Drop()
    {
        foreach (Transform child in heldObject)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        transform.GetChild(0).gameObject.SetActive(true);
        heldObject.gameObject.layer = 0;
        heldObjectRb.useGravity = true;

        heldObjectCollider = null;
        heldObjectRb = null;
        heldObject = null;

        isCarrying = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void NullifyRbForces()
    {
        heldObjectRb.angularVelocity = Vector3.zero;
        heldObjectRb.velocity = Vector3.zero;
        heldObjectRb.useGravity = false;
    }
    
    void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(0, 1, 0.5f, 0.5f);

        if (heldObjectCollider)
        {
            Vector3 tempPos = heldObject.position;
            tempPos.y = heldObjectCollider.bounds.center.y;
            Gizmos.DrawCube(tempPos, heldObjectCollider.bounds.size);
        }
    }
}
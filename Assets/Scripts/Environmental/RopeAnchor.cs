using System.Collections;
using UnityEngine;

public class RopeAnchor : MonoBehaviour
{
    private Rigidbody rb;
    private bool oneShot = true;
    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
    }
    
    private void OnCollisionStay(Collision col)
    {
        if (oneShot)
        {
            Rigidbody collidedRb = col.rigidbody;
            if (collidedRb)
            {
                FixedJoint joint = transform.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = collidedRb;
                collidedRb.isKinematic = false;
                rb.freezeRotation = false;
                rb.isKinematic = false;
            }

            oneShot = false;
        }
    }
}

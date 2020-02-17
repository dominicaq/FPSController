using UnityEngine;
using UnityEngine.AddressableAssets;

public class RopeAnchor : MonoBehaviour
{
    private Rigidbody rb;
    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
    }

    private void Update()
    {
        Destroy(this);
    }

    private void OnCollisionStay(Collision col)
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
    }
}

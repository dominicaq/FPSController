using System.Collections;
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
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //play your sound
        yield return new WaitForSeconds(1); //waits 3 seconds
        Destroy(this); //this will work after 3 seconds.
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

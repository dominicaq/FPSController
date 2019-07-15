using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionImpulse : MonoBehaviour
{
    public float strength = 25;
    public float explosionRadius = 5;
    private float linger = 0;
    private Vector3 direction;
    private CharacterController playerMovement;
    private PlayerForce forceModifier;
    private SphereCollider myCollider;

    // Audio
    private AudioSource audioData;

    private void Awake() 
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.radius = explosionRadius;

        audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        linger += Time.deltaTime;
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Player" && linger <= 0.02f)
        {
            playerMovement = hit.GetComponent<CharacterController>();
            forceModifier = hit.GetComponent<PlayerForce>();

            float forceAmp = Vector3.Distance(hit.transform.position, transform.position);

            // Zero for new force
            forceModifier.impact = Vector3.zero;
            direction = transform.position - hit.transform.position;
            direction.y /= 2;
            direction.Normalize();

            // Finalize
            forceModifier.AddForce(direction * strength);
        }
    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

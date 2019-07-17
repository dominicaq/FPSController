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

        // Linger long enough for sound to finish playing
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider hit)
    {
        playerMovement = hit.GetComponent<CharacterController>();
        forceModifier = hit.GetComponent<PlayerForce>();

        // Direction of explosion
        direction = transform.position - hit.transform.position;
        direction.y /= 2;
        direction.Normalize();

        // Finalize
        forceModifier.AddForce(direction * strength);
    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

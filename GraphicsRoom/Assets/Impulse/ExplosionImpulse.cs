using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionImpulse : MonoBehaviour
{
    public float strength = 30;
    public float explosionRadius = 5;
    public float activeDuration = 0.1f;
    private bool active = true;
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
        StartCoroutine("DurationActive");
        Destroy(gameObject, audioData.clip.length);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if(active)
        {
            playerMovement = hit.GetComponent<CharacterController>();
            forceModifier = hit.GetComponent<PlayerForce>();
            
            // Direction of explosion
            direction = transform.position - hit.transform.position;

            // Finalize
            if(forceModifier != null)
                forceModifier.AddForce(direction * strength);
        }
    }

    IEnumerator DurationActive()
    {
        active = true;
        yield return new WaitForSeconds(activeDuration);
        active = false;
    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

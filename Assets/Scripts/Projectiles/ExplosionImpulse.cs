using System.Collections;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class ExplosionImpulse : MonoBehaviour
{
    [Header("Properties")]
    public float strength = 30;
    public float explosionRadius = 5;
    public float lingerDuration = 0.1f;
    
    private bool isLingering = true;
    private Vector3 direction;
    private PlayerForce forceModifier;
    private SphereCollider myCollider;

    // Audio
    private AudioSource audioData;

    private void Start() 
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.radius = explosionRadius;

        audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        // Linger long enough for sound to finish playing
        StartCoroutine(Linger());
        StartCoroutine(DestroyObject());
    }

    private void OnTriggerEnter(Collider hit)
    {
        if(isLingering)
        {
            forceModifier = hit.GetComponent<PlayerForce>();
            
            // Direction of explosion
            direction = transform.position - hit.transform.position;

            // Finalize
            if(forceModifier != null)
                forceModifier.AddForce(direction * strength);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(audioData.clip.length);
        Addressables.ReleaseInstance(gameObject);
    }

    IEnumerator Linger()
    {
        isLingering = true;
        yield return new WaitForSeconds(lingerDuration);
        isLingering = false;
    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

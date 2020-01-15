using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Projectiles
{
    public class ExplosionImpulse : MonoBehaviour
    {
        [Header("Properties")] 
        public float strength = 15;
        public float rbStrength = 500;
        public float explosionRadius = 5;
        public float lingerDuration = 0.1f;

        private bool isLingering = true;
        private Vector3 direction;
        
        private PlayerForce forceModifier;
        private SphereCollider myCollider;
        private AudioSource audioData;

        private void Start()
        {
            myCollider = GetComponent<SphereCollider>();
            myCollider.radius = explosionRadius;

            audioData = GetComponent<AudioSource>();
            audioData.Play(0);

            // Linger for explosion code
            StartCoroutine(Linger());
            
            // Linger for sound
            StartCoroutine(DestroyObject());
        }
        
        private void OnTriggerEnter(Collider hit)
        {
            if (isLingering)
            {
                // Direction of explosion
                direction = transform.position - hit.transform.position;
                
                // Add force to players and rigid bodies
                forceModifier = hit.GetComponent<PlayerForce>();
                if (forceModifier)
                    forceModifier.AddForce(direction * strength);

                Rigidbody hitRb = hit.GetComponent<Rigidbody>();
                if (hitRb)
                    hitRb.AddForce(-direction * rbStrength);
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
            Gizmos.color = new Color(1,0.5f,0, 0.5f);
            Gizmos.DrawSphere(transform.position, explosionRadius);
        }
    }
}
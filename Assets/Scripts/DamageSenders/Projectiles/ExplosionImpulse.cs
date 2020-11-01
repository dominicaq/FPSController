using System.Collections;
using UnityEngine;

namespace Projectiles
{
    public class ExplosionImpulse : MonoBehaviour
    {
        public float radius = 5;
        public float strength = 15;
        public float rbStrength = 200;
        
        private AudioSource audioSrc;

        private void Start()
        {
            audioSrc = GetComponent<AudioSource>();
            
            Explode();
            StartCoroutine(DestroyObject());
        }

        private void Explode()
        {
            audioSrc.Play(0);
            Collider[] collArr = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in collArr)
            {
                Vector3 dir = transform.position - hit.transform.position;

                PlayerForce forceModifier = hit.GetComponent<PlayerForce>();
                if (forceModifier)
                    forceModifier.AddForce(dir * strength);

                Rigidbody hitRb = hit.GetComponent<Rigidbody>();
                if (hitRb)
                    hitRb.AddForce(-dir * rbStrength);
            }
        }

        private IEnumerator DestroyObject()
        {
            yield return new WaitForSeconds(audioSrc.clip.length);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1,0.5f,0, 0.5f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
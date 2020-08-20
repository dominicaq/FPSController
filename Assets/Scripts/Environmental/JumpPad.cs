using UnityEngine;

namespace Environmental
{
    public class JumpPad : MonoBehaviour
    {
        public float strength = 2;
    
        [Header("Components")]
        private AudioSource m_AudioSource;
        
        private void Start() 
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerForce forceModifier = other.GetComponent<PlayerForce>();
            if (forceModifier)
            {
                forceModifier.velocity = Vector3.zero;
                forceModifier.AddYForce(strength);

                m_AudioSource.Play(0);
            }
        }
    } 
}


using UnityEngine;
using Controller;
using UnityEngine.AddressableAssets;

namespace Environmental
{
    public class JumpPad : MonoBehaviour
    {
        public float strength = 2;
    
        [Header("Components")]
        private PlayerForce forceModifier;
        private AudioSource audioData;
        private AssetReference jumpSfx;
        private void Start() 
        {
            audioData = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                forceModifier = other.GetComponent<PlayerForce>();

                // Zero for new force
                forceModifier.velocity = Vector3.zero;
            
                forceModifier.AddForce(strength);
                audioData.Play(0);
            }
        }
    } 
}


using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private bool enableJumpPad = true;
    [SerializeField] private float strength = 2;
    private CharacterController playerMovement;
    private PlayerForce forceModifier;
    private PlayerController playerConditions;

    // Audio
    private AudioSource audioData;

    private void Start() 
    {
        audioData = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enableJumpPad)
        {
            playerMovement = other.GetComponent<CharacterController>();
            playerConditions = other.GetComponent<PlayerController>();
            forceModifier = other.GetComponent<PlayerForce>();

            // Zero for new force
            forceModifier.velocity = Vector3.zero;
            
            forceModifier.AddForce(strength);
            audioData.Play(0);
        }
    }
}

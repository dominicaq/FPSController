using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float strength = 2;
    private CharacterController playerMovement;
    private PlayerForce forceModifier;
    private PlayerController playerConditions;

    // Audio
    private AudioSource audioData;

    private void Awake() 
    {
        audioData = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerMovement = other.GetComponent<CharacterController>();
            playerConditions = other.GetComponent<PlayerController>();
            forceModifier = other.GetComponent<PlayerForce>();

            // Zero for new force
            forceModifier.impact = Vector3.zero;

            if (transform.eulerAngles != Vector3.zero)
            {
                // Old code broken
                // If jumpPad is rotated, launch player accordingly
            }

            forceModifier.AddForce(strength);
            audioData.Play(0);
        }
    }
}

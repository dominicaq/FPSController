using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float strength = 2;
    private CharacterController playerMovement;
    private PlayerForce forceModifier;

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
            forceModifier = other.GetComponent<PlayerForce>();

            // Zero for new force
            forceModifier.impact = Vector3.zero;

            if (transform.eulerAngles != Vector3.zero)
            {
                // Old code broken
                // If jumpPad is rotated, launch player accordingly
            }

            forceModifier.AddUpwardForce(strength);
            audioData.Play(0);
        }
    }
}

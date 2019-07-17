using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Camera Settings
    [Header("Spectator")]    
    public bool enableSpectator = false;
    public float spectatorMovement = 5.0f;

    [Header("General Settings")]
    public bool lockCursor = true;
    public float playerFOV = 90;
    public float cameraHeight = 0.5f;
    private Camera cam;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    // Crouching
    public bool cameraCanTransition = false;
    private float tParam = 0.0f;

    // Character Controller
    private Transform player;
    private PlayerController playerCC;
    private CameraShake camShake;

    void Start()
    {
        // Camera properties
        cam = transform.GetComponent<Camera>();
        camShake = transform.GetComponent<CameraShake>();

        // Player controller
        player = transform.parent;
        playerCC = player.GetComponent<PlayerController>();

        if(lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        // Shake effects
        float shakeValue = camShake.zShake;
        float punchValue = camShake.yPunch;

        cam.fieldOfView = playerFOV;

        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

        // If player is looking up, reverse punch value
        if (pitch <= -90)
        {
            punchValue = -punchValue;
        }

        pitch = Mathf.Clamp(pitch, -90 + punchValue, 90);
        transform.eulerAngles = new Vector3(pitch - punchValue, yaw, shakeValue);

        if (enableSpectator)
        {
            transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * spectatorMovement, 0, 0);
            transform.Translate(0, 0, Input.GetAxis("Vertical") * Time.deltaTime * spectatorMovement);
        }
        else
        {
            // Cameraheight is a lerped float
            Vector3 desiredHeight = transform.localPosition;
    
            desiredHeight.y = cameraHeight;
            transform.localPosition = desiredHeight;
        }

        // Crouching
        if (cameraCanTransition)
            AdjustCamera();
    }

    private void AdjustCamera()
    {
        float stand = .2f;
        float crouch = .6f;
        
        tParam += playerCC.crouchRate * Time.fixedDeltaTime;
        if (playerCC.isCrouching)
        {
            cameraHeight = Mathf.Lerp(crouch, stand, tParam);
            
            if (cameraHeight == stand)
            {
                cameraCanTransition = false;
                tParam = 0;
            }
        }
        else
        {
            cameraHeight = Mathf.Lerp(stand, crouch, tParam);

            if (cameraHeight == crouch)
            {
                cameraCanTransition = false;
                tParam = 0;
            }
        }
    }
}

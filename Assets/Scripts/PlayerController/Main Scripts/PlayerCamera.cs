using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    // Camera Settings
    [Header("General Settings")]
    [SerializeField] [Range(75, 90)] private float p_FieldOfView = 90;
    [SerializeField] private float cameraHeight = 0.5f;
    public bool lockCursor = true;
    private Camera cam;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    // Character Controller
    private Transform player;
    private PlayerController playerCC;
    private CameraShake camShake;

    private void Start()
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

    private void Update()
    {
        // Shake effects
        float shakeValue = camShake.zShake;
        float punchValue = camShake.yPunch;

        // 75-90 FOV only
        cam.fieldOfView = p_FieldOfView;

        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

        // If player is looking up, reverse punch value
        if (isLookingUp(90))
        {
            punchValue = -punchValue;
        }

        pitch = Mathf.Clamp(pitch, punchValue - 90, 90);
        transform.eulerAngles = new Vector3(pitch - punchValue, yaw, shakeValue);

        // Cameraheight is a lerped float
        Vector3 desiredHeight = transform.localPosition;
    
        desiredHeight.y = cameraHeight;
        transform.localPosition = desiredHeight;
    }

    public float getPitch()
    {
        return Mathf.Abs(pitch);
    }

    public bool isLookingUp(float angle)
    {
        return pitch <= -angle ? true : false;
    }

    public bool isLookingDown(float angle)
    {
        return pitch >= angle ? true : false;
    }
}

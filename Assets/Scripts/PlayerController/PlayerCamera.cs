using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    // Camera Settings
    [Header("Properties")]
    [SerializeField] [Range(75, 90)] private float fieldOfView = 90;
    [SerializeField] private float cameraHeight = 0.5f;
    public bool lockCursor = true;
    
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    [Header("Components")]
    private Camera cam;
    private CameraShake camShake;

    private void Start()
    {
        // Camera properties
        cam = transform.GetComponent<Camera>();
        camShake = transform.GetComponent<CameraShake>();
        
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
        cam.fieldOfView = fieldOfView;

        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

        // If player is looking up, reverse punch value
        if (IsLookingUp(90))
        {
            punchValue = -punchValue;
        }

        pitch = Mathf.Clamp(pitch, punchValue - 90, 90);
        transform.eulerAngles = new Vector3(pitch - punchValue, yaw, shakeValue);
        
        Vector3 desiredHeight = transform.localPosition;
        desiredHeight.y = cameraHeight;
        transform.localPosition = desiredHeight;
    }

    public float GetPitch()
    {
        return Mathf.Abs(pitch);
    }

    public bool IsLookingUp(float angle)
    {
        return pitch <= -angle ? true : false;
    }

    public bool IsLookingDown(float angle)
    {
        return pitch >= angle ? true : false;
    }
}

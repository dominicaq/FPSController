using System;
using Managers;
using UnityEngine;

// Note: Nothing but camera modifiers and input should edit rotation

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    // Camera Settings
    [Header("Properties")]  
    [Range(75, 90)] public float fieldOfView = 90;

    [Header("Camera Axis")] 
    [NonSerialized]
    public float pitch,
        yaw,
        roll;

    [Header("Mouse Settings")] [SerializeField]
    private float mouseSensitivity = 2.5f;

    [Header("Components")] 
    private Camera cameraComponent;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void CameraInput()
    {
        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
    }
    
    private void Update()
    {
        if(!GameState.isPaused)
            CameraInput();
        
        // FOV
        cameraComponent.fieldOfView = fieldOfView;
        pitch = Mathf.Clamp(pitch, -90, 90);

        Vector3 inputDir = new Vector3(pitch, yaw, roll);
        transform.eulerAngles = inputDir;
    }

    /// <summary> Get camera pitch </summary>
    /// <returns>pitch</returns>
    public float GetPitch()
    {
        return pitch;
    }

    // Maybe revise these two functions
    /// <summary> Using a specified angle is the camera looking up? </summary>
    public bool IsLookingUp(float angle)
    {
        return pitch <= -angle;
    }

    /// <summary> Using a specified angle is the camera looking down? </summary>
    public bool IsLookingDown(float angle)
    {
        return pitch >= angle;
    }
}

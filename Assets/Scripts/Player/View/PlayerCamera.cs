using System;
using UnityEngine;
using Managers;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]  
    [Range(0, 120)] public float fieldOfView = 90;
    public float mouseSensitivity = 2.5f;

    [Header("Camera Axis")] 
    [NonSerialized] public float pitch, yaw, roll;
    [NonSerialized] public float turnRate;
    private Camera m_GameCamera;

    private void Start()
    {
        turnRate                 = mouseSensitivity;
        m_GameCamera             = GetComponent<Camera>();
        m_GameCamera.fieldOfView = fieldOfView;
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(pitch, yaw, roll);

        if(GameState.isPaused)
            return;

        MouseInput();
    }

    private void MouseInput()
    {
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90, 90);

        yaw += turnRate * Input.GetAxis("Mouse X");
    }
}

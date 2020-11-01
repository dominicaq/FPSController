using System;
using UnityEngine;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson,
    FreeCamera,
    NONE
};

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [System.NonSerialized] public Camera cameraComponent;
    private Transform cameraPivot;

    [Header("Properties")]
    public float mouseSensitivity = 2.5f;
    [Range(0, 120)] public float fieldOfView = 90;
    public CameraMode currentMode = CameraMode.FirstPerson;
    private CameraMode previousMode;
    private Vector3 initLocalPosition;

    [Header("Input")]
    public Vector3 rawInput;
    public Vector3 cameraEuler;
    [NonSerialized] public float pitch, yaw, roll;
    [NonSerialized] public Ray centerOfScreenRay;

    [Header("Third Person")]
    public float collisionOffset = 0.2f;
    public Vector3 desiredPosition = new Vector3(0,1,-3);

    private void Awake()
    {
        previousMode      = CameraMode.NONE;
        cameraPivot       = transform.parent;
        cameraComponent   = GetComponent<Camera>();
        initLocalPosition = transform.localPosition;        
    }

    public void Move(Vector2 input)
    {
        rawInput = input;

        input *= 0.5f;
        input *= 0.1f;

        yaw += input.x * mouseSensitivity;
        pitch -= input.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -90, 90);

        cameraEuler = new Vector3(pitch, yaw, 0);
    }

    private void LateUpdate()
    {
        centerOfScreenRay = cameraComponent.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        cameraComponent.fieldOfView = fieldOfView;

        if(previousMode != currentMode)
            SwitchCameraMode(currentMode);

        if(currentMode == CameraMode.FirstPerson)
            FirstPersonMode();
        else if (currentMode == CameraMode.ThirdPerson)
            ThirdPersonMode();
    }

    public void SwitchCameraMode(CameraMode newCameraMode)
    {
        previousMode = newCameraMode;
        switch(newCameraMode)
        {
            case CameraMode.FirstPerson:
                ShowViewModel(true);
                break;
            case CameraMode.ThirdPerson:
                ShowViewModel(false);
                break;
            case CameraMode.FreeCamera:
                ShowViewModel(false);
                break;
        }
    }

    private void FirstPersonMode()
    {
        cameraPivot.eulerAngles = Vector3.zero;
        transform.localPosition = initLocalPosition;

        cameraEuler.z = roll;
        transform.eulerAngles = cameraEuler;
    }

    private void ThirdPersonMode()
    {
        Vector3 currentPos    = desiredPosition;
        Vector3 camDir        = cameraPivot.TransformPoint(desiredPosition) - cameraPivot.position;
        float desiredDistance = Vector3.Distance(desiredPosition, Vector3.zero);

        // Collision
        if (Physics.SphereCast(cameraPivot.position, collisionOffset, camDir, out RaycastHit hit, desiredDistance))
            currentPos = desiredPosition.normalized * (hit.distance - collisionOffset);

        cameraPivot.eulerAngles = cameraEuler;
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * 15f);
    }

    public void ShowViewModel(bool key)
    {
        transform.GetChild(0).gameObject.SetActive(key);
    }
}

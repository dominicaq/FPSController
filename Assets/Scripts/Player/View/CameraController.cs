using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 2.5f;
    [Range(0, 120)] public float fieldOfView = 90;
    private Vector3 m_SavedLocalPosition;

    [Header("Camera Axis")]
    public Vector3 rawInput;
    public Vector3 cameraEuler;
    [NonSerialized] public float pitch, yaw, roll;
    private Camera m_Camera;

    [Header("Third Person")]
    public bool enableThirdPerson = false;
    public float thirdpersonCollisionOffset = 0.2f;
    public Vector3 thirdpersonLocalPosition = new Vector3(0,0,-3);

    private void Start()
    {
        m_Camera             = GetComponent<Camera>();
        m_Camera.fieldOfView = fieldOfView;
        m_SavedLocalPosition = transform.localPosition;
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
        if(!enableThirdPerson)
        {
            FirstPerson();
            SetViewModel(true);
        }
        else
        {
            ThirdPerson();
            SetViewModel(false);
        }

    }

    public void FirstPerson()
    {
        transform.localPosition = m_SavedLocalPosition;

        cameraEuler.z = roll;
        transform.eulerAngles = cameraEuler;
    }

    public void ThirdPerson()
    {
        // Collision
        Vector3 currentPos = thirdpersonLocalPosition;
        
        Vector3 dirTmp = transform.parent.TransformPoint(thirdpersonLocalPosition) - transform.parent.position;
        if (Physics.SphereCast(transform.parent.parent.position, thirdpersonCollisionOffset, dirTmp, out RaycastHit hit, Vector3.Distance(thirdpersonLocalPosition, Vector3.zero)))
        {
            currentPos = thirdpersonLocalPosition.normalized * (hit.distance - thirdpersonCollisionOffset);
        }

        transform.parent.eulerAngles = cameraEuler;
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * 15f);
    }

    public void SetViewModel(bool key)
    {
        transform.GetChild(0).gameObject.SetActive(key);
    }
}

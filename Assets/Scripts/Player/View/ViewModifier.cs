using System;
using UnityEngine;

public class ViewModifier : MonoBehaviour
{
    [Header("Enable/Disable")]
    public bool enableCameraSway = false;
    public bool enableWeaponOscillate = true;
    public bool enableWeaponSway = true;

    #region View Model
    public Transform viewModel;

    [Header("View Model Oscillate")] 
    public float smoothWalking = 0.3f;
    public float oscillateDistance = 10;
    private float smoothWalkVelocity;
    private Vector3 initLocalPos;

    [Header("View Model Sway")] 
    public float swayIntensityX = 5;
    public float swayIntensityY = 5;

    #endregion

    #region Camera
    
    [Header("Camera Sway")]
    public float smoothSway = 0.1f;
    public float swayAngle = 1;
    private float smoothSwayVelocity;

    #endregion

    #region Components
    private PlayerController m_PlayerController;
    private CameraController cameraController;

    #endregion
    
    void Start()
    {
        initLocalPos       = viewModel.localPosition;
        m_PlayerController = GetComponentInParent<PlayerController>();
        cameraController   = GetComponent<CameraController>();
    }
    
    void Update()
    {
        if(enableWeaponOscillate)
            WeaponOscillate();

        if(enableWeaponSway)
            WeaponSway();

        if(enableCameraSway)
            CameraSway();
    }

    private void WeaponOscillate()
    {
        if (m_PlayerController.isCrouching)
            smoothWalkVelocity = 0;
        
        if (m_PlayerController.inputVector.z != 0)
            initLocalPos.z = Mathf.SmoothDamp(initLocalPos.z,Mathf.Sin(Time.time * 2.5f) / oscillateDistance, ref smoothWalkVelocity, smoothWalking);
        else
            initLocalPos.z = Mathf.SmoothDamp(initLocalPos.z, 0, ref smoothWalkVelocity, smoothWalking);

        viewModel.localPosition = initLocalPos;
    }
    
    private void WeaponSway()
    {
        float inputX = Mathf.Clamp(cameraController.rawInput.x, -2, 2);
        float inputY = cameraController.rawInput.y;
        
        Quaternion swayX = Quaternion.AngleAxis(swayIntensityX * inputX, Vector3.down);
        Quaternion swayY = Quaternion.AngleAxis(swayIntensityY * inputY, Vector3.right);
        Quaternion targetRot = swayX;

        if (cameraController.pitch >= -89 && cameraController.pitch <= 89)
            targetRot *= swayY;
        
        viewModel.localRotation = Quaternion.Lerp(viewModel.localRotation, targetRot, Time.deltaTime * (cameraController.mouseSensitivity / 1.5f)); 
    }

    private void CameraSway()
    {
        float input = m_PlayerController.inputVector.x;
        float targetRoll = input * swayAngle;
        
        if(input < 0 || input > 0)
            cameraController.roll =  Mathf.SmoothDamp(cameraController.roll,  -targetRoll, ref smoothSwayVelocity, smoothSway);
        else
            cameraController.roll = Mathf.SmoothDamp(cameraController.roll, 0, ref smoothSwayVelocity, smoothSway);
    }
}

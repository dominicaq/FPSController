using System;
using UnityEngine;

public class ViewModifier : MonoBehaviour
{
    //[Header("Camera Shake")]
    [Header("Camera Sway")]
    public float smooth = 0.1f;
    public float angle = 1;
    private float smoothVelocity;

    [Header("Bobbing")]
    public float smoothWalking = 0.3f;
    public float bobDistance = 10;
    private float smoothWalkVelocity;
    private Vector3 currentPos;
    private Transform movingObject;

    [Header("View Model")] 
    public float swayIntensityX = 10;
    public float swayIntensityY = 10;
    public float swayTime = 5;
    private PlayerController m_PlayerController;
    private PlayerCamera m_pCamera;
    
    void Start()
    {
        movingObject       = transform.GetChild(0);
        currentPos         = movingObject.localPosition;
        m_PlayerController = GetComponentInParent<PlayerController>();
        m_pCamera          = GetComponent<PlayerCamera>();
    }
    
    void Update()
    {
        WeaponOscillate();
        WeaponSway();

        CameraSway();
    }

    private void WeaponOscillate()
    {
        if (m_PlayerController.isCrouching)
            smoothWalkVelocity /= 1.2f;
        
        if (Input.GetAxis("Vertical") != 0)
            currentPos.z = Mathf.SmoothDamp(currentPos.z,Mathf.Sin(Time.time * 2.5f) / bobDistance, ref smoothWalkVelocity, smoothWalking);
        else
            currentPos.z = Mathf.SmoothDamp(currentPos.z, 0, ref smoothWalkVelocity, smoothWalking);

        movingObject.localPosition = currentPos;
    }
    
    private void WeaponSway()
    {
        float inputX = Mathf.Clamp(Input.GetAxis("Mouse X"), -2, 2);
        float inputY = Input.GetAxis("Mouse Y");
        
        Quaternion swayX = Quaternion.AngleAxis(swayIntensityX * inputX, Vector3.down);
        Quaternion swayY = Quaternion.AngleAxis(swayIntensityY * inputY, Vector3.right);
        Quaternion targetRot = swayX;

        if (m_pCamera.pitch >= -89 && m_pCamera.pitch <= 89)
            targetRot *= swayY;
        
        movingObject.localRotation = Quaternion.Lerp(movingObject.localRotation, targetRot, Time.deltaTime * swayTime); 
    }

    private void CameraSway()
    {
        float input = Input.GetAxis("Mouse X");
        float targetRoll = input * angle;
        
        if(input < 0 || input > 0)
            m_pCamera.roll =  Mathf.SmoothDamp(m_pCamera.roll,  -targetRoll, ref smoothVelocity, smooth);
        else
            m_pCamera.roll = Mathf.SmoothDamp(m_pCamera.roll, 0, ref smoothVelocity, smooth);
    }
}

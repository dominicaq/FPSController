using System;
using Controller;
using UnityEngine;

public class viewWeaponBob : MonoBehaviour
{
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
    private Vector3 smoothSway;

    private PlayerController playerController;
    private PlayerCamera myCamera;

    private float bobStep;
    void Start()
    {
        movingObject = transform.GetChild(0);
        currentPos = movingObject.localPosition;
        playerController = transform.parent.GetComponent<PlayerController>();
        myCamera = transform.GetComponent<PlayerCamera>();
    }
    
    // Update is called once per frame
    void Update()
    {
        InventoryBob();
        InventorySway();
    }

    private void InventoryBob()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");

        if (playerController.isCrouching)
            smoothWalkVelocity /= 1.2f;
        
        if (forwardInput < 0 || forwardInput > 0)
        {
            currentPos.z = Mathf.SmoothDamp(currentPos.z,Mathf.Sin(Time.time * 2.5f) / bobDistance, ref smoothWalkVelocity, smoothWalking);
        }
        else
        {
            currentPos.z = Mathf.SmoothDamp(currentPos.z,0, ref smoothWalkVelocity, smoothWalking);
        }

        movingObject.localPosition = currentPos;
    }
    
    private void InventorySway()
    {
        float inputX = Mathf.Clamp(Input.GetAxis("Mouse X"), -2, 2),
              inputY = Input.GetAxis("Mouse Y");
        
        Quaternion swayX = Quaternion.AngleAxis(swayIntensityX * inputX, Vector3.down);
        Quaternion swayY = Quaternion.AngleAxis(swayIntensityY * inputY, Vector3.right);
        Quaternion targetRot = swayX;

        if (!myCamera.IsLookingUp(90) && !myCamera.IsLookingDown(90))
        {
            targetRot *= swayY;
        }
        
        movingObject.localRotation = Quaternion.Lerp(movingObject.localRotation, targetRot, Time.deltaTime * swayTime);
      
    }
}

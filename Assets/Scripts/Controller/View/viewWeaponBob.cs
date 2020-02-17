using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class viewWeaponBob : MonoBehaviour
{
    [Header("Bobbing")]
    public float smoothWalking = 0.3f;
    public float bobDistance = 0.1f;
    private float smoothWalkVelocity;
    private Vector3 currentPos;
    private bool flip;
    private Transform movingObject;

    [Header("View Model")] 
    public float swayIntensityX = 10;
    public float swayIntensityY = 10;
    public float swayTime = 5;
    private Vector3 smoothSway;

    private PlayerController controller;
    private PlayerCamera myCamera;
    void Start()
    {
        movingObject = transform.GetChild(0);
        currentPos = movingObject.localPosition;
        controller = transform.parent.GetComponent<PlayerController>();
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
        float vert = Input.GetAxisRaw("Vertical");
        currentPos.z = Mathf.Clamp(currentPos.z, bobDistance * 0.1f, bobDistance);

        if (controller.isCrouching)
        {
            smoothWalkVelocity = smoothWalkVelocity / 1.2f;
        }

        if (vert != 0 && !flip)
        {
            currentPos.z = Mathf.SmoothDamp(currentPos.z,bobDistance, ref smoothWalkVelocity, smoothWalking);
            if (currentPos.z >= bobDistance * .9f)
            {
                flip = true;
            }
        }
        else
        {
            currentPos.z = Mathf.SmoothDamp(currentPos.z,0, ref smoothWalkVelocity, smoothWalking);
            if (currentPos.z <= bobDistance * 0.1f)
            {
                flip = false;
            }
        }
        
        movingObject.localPosition = currentPos;
    }
    
    private void InventorySway()
    {
        float inputX = Input.GetAxis("Mouse X"),
              inputY = Input.GetAxis("Mouse Y");

        // TODO: random teleporting on way, check it later
        inputX = Mathf.Clamp(inputX, -swayIntensityX, swayIntensityX);
        
        float currentSwayTime = controller.isCrouching ? swayTime / 2f: swayTime;
        
        Quaternion swayX = Quaternion.AngleAxis(-swayIntensityX * inputX, Vector3.down);
        Quaternion swayY = Quaternion.AngleAxis(swayIntensityY * inputY, Vector3.right);
        Quaternion targetRot = swayX;

        if (!myCamera.IsLookingUp(90) && !myCamera.IsLookingDown(90))
        {
            targetRot *= swayY;
        }
        
        movingObject.localRotation = Quaternion.Lerp(movingObject.localRotation, targetRot, Time.deltaTime * currentSwayTime);
    }
}

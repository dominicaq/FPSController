using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewWeaponBob : MonoBehaviour
{
    [Header("Properties")]
    // Bobbing
    public float smoothWalking = 0.3f;
    public float bobDistance = 0.2f;
    private float smoothWalkVelocity;
    // Swaying
    public float maxSway = 10f;
    
    private PlayerCamera cam;
    void Start()
    {
        cam = GetComponent<PlayerCamera>();
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = transform.localPosition;
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        
        if (vert != 0)
        {
            currentPos.z = Mathf.PingPong(-Time.time * 0.1f, bobDistance);
        }
        else
        {
            currentPos.z = Mathf.SmoothDamp(currentPos.z,0, ref smoothWalkVelocity, smoothWalking);
        }

        ViewSway();
        transform.localPosition = currentPos;
    }

    private void ViewSway()
    {
        float x = Input.GetAxis("Mouse X"),
              y = Input.GetAxis("Mouse Y");
    }
}

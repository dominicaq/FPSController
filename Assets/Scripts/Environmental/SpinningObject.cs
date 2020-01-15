using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    public float speed = 1.0f;
    [Range(0.001f, 1)] public float acceleration = 1.0f;
    public bool active = true;
    private float currentSpeed;
    void Update()
    {
        currentSpeed = Mathf.Clamp(currentSpeed, 0, speed);

        if (active)
            currentSpeed += acceleration;
        else
            currentSpeed -= acceleration;
        
        // Time is already applied within function
        transform.Rotate( currentSpeed, 0,  0);
    }
}

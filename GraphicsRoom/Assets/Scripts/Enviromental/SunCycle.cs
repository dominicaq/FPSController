using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCycle : MonoBehaviour
{
    public bool turnOn;
    public float turnRate = 2.0f;
    private float rate = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        if(turnOn)
        {
            rate += Time.deltaTime * turnRate;
            
            Vector3 v = new Vector3(rate, -45, -45);
            transform.localRotation = Quaternion.Euler(v);
        }
    }
}

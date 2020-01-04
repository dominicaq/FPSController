using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environmental
{
    public class WindForce : MonoBehaviour
    {
        public float currentIntensity = 1.0f;
        private float oscillateValue;
        public float min = 0;
        public float max = 5f;
        private float clock;
        
        private void Update()
        {
            clock += Time.deltaTime;
            oscillateValue = Mathf.Sin(Mathf.PI * clock);
            currentIntensity = Mathf.Clamp(oscillateValue, min, max);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (enabled && other.attachedRigidbody)
            {
                Vector3 triggerRot = transform.rotation * Vector3.forward;
                other.attachedRigidbody.AddForce(transform.forward + triggerRot * currentIntensity);
            }
        }
    }
}
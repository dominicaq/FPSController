using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public Transform startPoint, endPoint, pointPrefab;
    
    [Header("Line Properties")]
    public int resolution = 5;
    public float radius = 0.5f;
    private LineRenderer lineRenderer;

    [Header("Stiffness")] 
    public float maxStiffness = 30; 
    public float minStiffness = 5;
    private float currentStiffness = 30;
    private float savedDist;
    
    private Transform[] points;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        lineRenderer.widthMultiplier = radius;
        pointPrefab.localScale = Vector3.one * radius;

        // Set Joints
        points = new Transform[resolution];
        for (int i = 0; i < resolution; i++)
        {
            points[i] = Instantiate(pointPrefab, transform);
            float p = i / (resolution - 1f);
            points[i].position = Vector3.Lerp(startPoint.position, endPoint.position, p);
            
            if (i != 0)
            {
                SpringJoint sj = points[i].gameObject.AddComponent<SpringJoint>();
                sj.connectedBody = points[i-1].GetComponent<Rigidbody>();
                sj.spring = currentStiffness;
                sj.damper = 0.5f;
                sj.anchor = Vector3.zero;
            }
        }

        savedDist = Vector3.Distance(points[0].position, points[resolution - 1].position);
        points[0].GetComponent<Rigidbody>().isKinematic = true;
        points[resolution-1].GetComponent<Rigidbody>().isKinematic = true;
    }

    //https://forum.unity.com/threads/draw-cylinder-between-2-points.23510/
    private void Update()
    {
        currentStiffness = maxStiffness; //debug
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (i != 0)
            {
                SpringJoint sj = points[i].GetComponent<SpringJoint>();
                sj.spring = currentStiffness;
            }

            lineRenderer.SetPosition(i, points[i].position);
        }

        float currentDistance = Vector3.Distance(points[0].position, points[resolution - 1].position);
    }
}

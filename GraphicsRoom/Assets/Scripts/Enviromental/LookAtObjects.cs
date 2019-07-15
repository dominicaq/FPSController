using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObjects : MonoBehaviour
{
    public int selection = 0;
    public float turnRate = 1.0f, waitTime = 1.0f;
    public Transform[] targets;

    void Start()
    {
        InvokeRepeating("Cycle", 0, waitTime);
    }
    void Update()
    {
        Vector3 lTargetDir = targets[selection].position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * (turnRate * 10));
    }

    void Cycle()
    {
        if(selection == targets.Length-1)
        {
            selection = 0;
        }
        else
        {
            selection++;
        }
    }
}

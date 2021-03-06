﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environmental
{
    public class LookAtObjects : MonoBehaviour
    {
        [SerializeField] private int selection = 0;
        [SerializeField] private float turnRate = 1.0f, waitTime = 1.0f;
        [SerializeField] private Transform[] targets = new Transform[0];

        void Start()
        {
            InvokeRepeating("Cycle", 0, waitTime);
        }

        void Update()
        {
            Vector3 lTargetDir = targets[selection].position - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir),
                Time.deltaTime * (turnRate * 10));
        }

        void Cycle()
        {
            if (selection == targets.Length - 1)
            {
                selection = 0;
            }
            else
            {
                selection++;
            }
        }
    }
}
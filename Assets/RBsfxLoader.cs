﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBsfxLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("IgnorePlayer"), true);
    }
}

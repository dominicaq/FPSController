using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Unit : MonoBehaviour
{
    public string unit_name;
    public int unit_level, unit_health;

    void Start(){
         unit_name = "DEFAULT";
         unit_level = 1;
         unit_health = 100;
    }
}



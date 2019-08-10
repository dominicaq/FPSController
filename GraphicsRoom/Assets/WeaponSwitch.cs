using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedItem = 0;
    [SerializeField] private int[] playerInventory;

    void Start()
    {
       playerInventory = new int[CountItems(transform)];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
        {
            if(selectedItem >= playerInventory.Length-1)
                selectedItem = 0;
            else
                selectedItem++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
        {
            if (selectedItem <= 0)
                selectedItem = playerInventory.Length-1;
            else
                selectedItem--;
        }   
    }

    int CountItems(Transform a)
    {
        int childCount = 0;
        foreach (Transform b in a)
        {
            Debug.Log("Child: "+ b);
            childCount ++;
            childCount += CountItems(b);
        }
        return childCount;
    }
}

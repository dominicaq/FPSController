using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSystem : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = new RaycastHit();
        Ray pingRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Input.GetKeyDown(KeyCode.F) && Physics.Raycast(pingRay, out hit))
        {
            Debug.Log(hit.collider.name);
        }        
    }
}

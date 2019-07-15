using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevGun : MonoBehaviour
{
    public float recoilAmount = 3f;
    public GameObject projectile;
    CameraShake recoil;

    void Start()
    {
        recoil = transform.parent.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        // Shoot
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out RaycastHit hit, 1000f))
                Instantiate(projectile, hit.point, Quaternion.identity);

            recoil.InduceAimPunch(recoilAmount);
        }
    }
}

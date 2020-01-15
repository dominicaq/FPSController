using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DevGun : MonoBehaviour
{
    public float maxDistance = 1000f;
    private Camera cam;
    private void Start()
    {
        cam = transform.parent.parent.GetComponent<Camera>();
    }
    private void Update()
    {
        // Shoot
        if(Input.GetMouseButtonDown(0))
        {
            int mask = ~LayerMask.GetMask("Player");
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance, mask))
                Addressables.InstantiateAsync("ImpulseSphere", hit.point, quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cam.transform.position, cam.transform.forward * maxDistance);
    }
}

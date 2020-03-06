using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DevGun : WeaponBase
{
    public float maxDistance = 1000f;
    private Camera cam;
    public LayerMask ignoreMask;
    public GameObject prefab;
    
    private void Start()
    {
        cam = transform.parent.parent.GetComponent<Camera>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    
    public override void Fire()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance, ignoreMask))
        {
            Addressables.InstantiateAsync("ImpulseSphere", hit.point, Quaternion.identity);
        }
    }
}

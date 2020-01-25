using Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DevGun : MonoBehaviour
{
    public float maxDistance = 1000f;
    private Camera cam;
    public LayerMask ignoreMask;
    private void Start()
    {
        cam = transform.parent.parent.GetComponent<Camera>();
    }
    private void Update()
    {
        if(!GameState.isPaused)
            Fire();
    }

    private void Fire()
    {
        // Shoot
        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance, ignoreMask))
            {
                Addressables.InstantiateAsync("ImpulseSphere", hit.point, quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(cam.transform.position, cam.transform.forward * maxDistance);
        }
    }
}

using Managers;
using UnityEngine;

public class DevGun : MonoBehaviour
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
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance, ignoreMask))
            {
                //FireImpulse(hit);
                FireBullets(hit);
            }
        }
    }
    
    public void FireBullets(RaycastHit hit)
    {
        if(hit.transform.gameObject.CompareTag("Unit"))
        {
            EntityHealth hp = hit.transform.GetComponent<EntityHealth>();

            if(hp)
                hp.SendDamage(25);
        }
    }

    public void FireImpulse(RaycastHit hit)
    {
        Instantiate(prefab, hit.point, Quaternion.identity);
    }
}

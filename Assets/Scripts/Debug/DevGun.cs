using UnityEngine;
using UnityEngine.AddressableAssets;

public class DevGun : MonoBehaviour
{
    public float maxDistance = 1000f;
    private CameraShake recoil;

    private void Start()
    {
        recoil = transform.parent.parent.GetComponent<CameraShake>();
    }
    private void Update()
    {
        // Shoot
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            recoil.InduceAimPunch(5);
            recoil.InduceStress(10);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                var go = AssetLibrary.Instantiate("ImpulseSphere");
                go.transform.position = hit.point;
                //AssetLibrary.InstantiateAtLocation("ImpulseSphere", hit.point, Quaternion.identity);
            }
        }
    }
}

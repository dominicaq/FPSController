using UnityEngine;

public class DevGun : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    private CameraShake recoil;

    void Start()
    {
        projectile = Resources.Load("Impulse/ImpulseSphere") as GameObject;
        recoil = transform.parent.parent.GetComponent<CameraShake>();
    }
    void Update()
    {
        // Shoot
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            recoil.InduceAimPunch(5);
            recoil.InduceStress(10);

            if (Physics.Raycast(ray,out RaycastHit hit, 1000f))
                GameObject.Instantiate(projectile, hit.point, Quaternion.identity);
        }
    }
}

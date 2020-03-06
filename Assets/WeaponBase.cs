using UnityEngine;

public struct WeaponStats
{
    public string name;
    public int damage;
    public float recoil;
    public int ammo;
    // More later
}

public class WeaponBase : MonoBehaviour
{
    public KeyCode shootKey = KeyCode.Mouse0;

    public virtual void Fire()
    {
        
    }
}

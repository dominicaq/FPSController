using UnityEngine;

public class DamageExplosion : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 1.0f;
    public float explosionDamage = 20.0f;
    public float countDownTime = 3.0f;

    public void RunTimer()
    {
        countDownTime -= Time.deltaTime;

        if(countDownTime <= 0.0f)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Collider[] collArr = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider entity in collArr)
        {
            if (Physics.Linecast(transform.position, entity.transform.position, out RaycastHit hit))
            {
                EntityHealth entityHP = hit.transform.GetComponent<EntityHealth>();

                if(entityHP)
                    entityHP.SendDamage(explosionDamage);
            }
        }
        
        Destroy(gameObject);
    }

    public virtual void OnDrawGizmos() 
    {
        Gizmos.color = new Color(1, 1.25f, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, explosionRadius);    
    }
}

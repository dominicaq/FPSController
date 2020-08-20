using System.Collections.Generic;
using UnityEngine;

public class EntityBreakable : EntityHealth
{
    public Transform breakModel;
    private DropTable m_Table;

    public void Start() 
    {
        m_Table = GetComponent<DropTable>();
    }

    public override void OnDeath()
    {
        if(m_Table)
        {
            List<GameObject> items = m_Table.DropItems();

            for(int i = 0; i < items.Count; i++)
            {
                Instantiate(items[i], transform.position, transform.rotation);
                Rigidbody rb = items[i].GetComponent<Rigidbody>();

                if(!rb)
                {
                    Vector3 force = new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f));
                    rb.AddForce(force * 200);
                }
            }
        }
        
        if(breakModel)
            Instantiate(breakModel, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDrone : MonoBehaviour
{
    public float desiredHoverHeight = 2.0f;
    private Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Hover(float hoverHeight)
	{
        if(Physics.Raycast(transform.position, Vector3.down * desiredHoverHeight, out RaycastHit hit, hoverHeight))
		{
            /*
            float hoverForce = Mathf.Max(hit.point.y + hoverHeight - transform.position.y, 0f);
            hoverForce = Mathf.Min(hoverForce, 1f);

            if(drone.position.y < hoverEval)
                rb_drone.AddForce(Vector3.up * hoverForce, ForceMode.VelocityChange);
            */
        }
	}

    private void OnDrawGizmos() 
    {
        Debug.DrawRay(transform.position, Vector3.down * desiredHoverHeight, Color.green);
    }
}

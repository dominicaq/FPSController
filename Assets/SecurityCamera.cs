using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public float range = 5;
    public Transform target;
    public Transform rotatingArm, head;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        // Draws a blue line from this transform to the target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(head.position, head.position + head.forward * range);
    }
}

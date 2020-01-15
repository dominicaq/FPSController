using UnityEngine;

/* Usage guide:
    To connect the rope to a rigidbody move one of the anchors
    to a desired position on the desired object and set the objects rigidbody to kinematic
 */

[RequireComponent(typeof(LineRenderer))]
public class RopeCreator : MonoBehaviour
{
    private Transform startPoint, endPoint, pointPrefab;
    
    [Header("Line Properties")]
    [Range(2, 100)]
    public int numPoints = 10;
    public float radius = 0.5f;
    private LineRenderer lineRenderer;

    [Header("Properties")] 
    public float spring = 10;
    public float damp = 100;
    private Transform[] points;
    
    private void Awake()
    {
        startPoint = transform.parent.GetChild(0);
        endPoint = transform.parent.GetChild(1);
        
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<MeshRenderer>().enabled = false;
        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.freezeRotation = true;
        pointPrefab = sphere.transform;
    }

    void Start()
    {
        GenerateRope();
    }

    public void GenerateRope()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numPoints;
        lineRenderer.widthMultiplier = radius;
        pointPrefab.localScale = Vector3.one * radius;

        // Set Joints
        SpringJoint sj;
        points = new Transform[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = Instantiate(pointPrefab, transform);
            float p = i / (numPoints - 1f);
            points[i].position = Vector3.Lerp(startPoint.position, endPoint.position, p);
            
            if (i != 0)
            {
                sj = points[i].gameObject.AddComponent<SpringJoint>();
                sj.connectedBody = points[i-1].GetComponent<Rigidbody>();
                sj.autoConfigureConnectedAnchor = false;
                sj.connectedAnchor = Vector3.zero;
                sj.spring = spring;
                sj.damper = damp;
            }
        }
        
        Transform p1 = points[0];
        Transform p2 = points[numPoints - 1];

        p1.gameObject.AddComponent<RopeAnchor>();
        p2.gameObject.AddComponent<RopeAnchor>();
    }

    //https://forum.unity.com/threads/draw-cylinder-between-2-points.23510/
    private void LateUpdate()
    {
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 anchor1 = transform.parent.GetChild(0).position;
        Vector3 anchor2 = transform.parent.GetChild(1).position;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(anchor1, Vector3.one * (radius * 1.1f));
        Gizmos.DrawCube(anchor2, Vector3.one * (radius * 1.1f));
        Debug.DrawLine(anchor1, anchor2);
    }
}

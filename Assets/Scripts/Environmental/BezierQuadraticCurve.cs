using UnityEngine;

public class BezierQuadraticCurve : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public int resolution = 12;
    public bool isStatic;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        if(isStatic)
            UpdateCurve();
    }

    void Update()
    {
        if(!isStatic)
            UpdateCurve();
    }

    // B(t) = (1-t)^2 * p1 + 2(1-t) * t * p1 + t^2 * p2;
    void UpdateCurve()
    {
		Vector3 position;
		for(int i = 0; i < resolution; i++)
		{
			float t = i / (resolution - 1.0f);
            
			position = (1.0f - t) * (1.0f - t) * 
            point1.position + 2.0f * (1.0f - t) * t * 
            point2.position + t * t * 
            point3.position;

			lineRenderer.SetPosition(i, position);
		}
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float ratio = 0; ratio < 1; ratio += 1.0f / resolution)
        {
            Gizmos.DrawLine(Vector3.Lerp(point1.position, point2.position, ratio), Vector3.Lerp(point2.position, point3.position, ratio));
        }
    }
}
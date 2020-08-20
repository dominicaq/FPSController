using UnityEngine;

public class BezierQuadraticCurve : MonoBehaviour
{
    public bool isStatic;
    public int resolution = 12;
    public Transform point1, point2, point3;
    private LineRenderer m_LineRenderer;

    // Use this for initialization
    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.positionCount = resolution;
        UpdateCurve();
    }

    void Update()
    {
        if(isStatic)
            return;

        UpdateCurve();
    }

    void UpdateCurve()
    {
		for(int i = 0; i < resolution; i++)
		{
			float t = i / (resolution - 1.0f);
			m_LineRenderer.SetPosition(i, BezierPoint(t));
		}
    }

    // B(t) = (1-t)^2 * p1 + 2(1-t) * t * p1 + t^2 * p2;
    private Vector3 BezierPoint(float t)
    {
        return (1.0f - t) * (1.0f - t) * point1.position + 2.0f * (1.0f - t) * t * point2.position + t * t * point3.position;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3[] positions = new Vector3[resolution];
		for(int i = 0; i < resolution; i++)
		{
			float t = i / (resolution - 1.0f);
            positions[i] = BezierPoint(t);
        }

        for(int i = 0; i < resolution - 1; i++)
            Gizmos.DrawLine(positions[i], positions[i+1]);
    }
}
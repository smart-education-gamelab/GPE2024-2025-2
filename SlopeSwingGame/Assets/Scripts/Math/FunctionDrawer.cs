using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FunctionDrawer : MonoBehaviour
{
    public enum FunctionType { Linear, Exponential }
    public float a = 1f; // Coefficient 'a'
    public float b = 0f; // Coefficient 'b'
    public float c = 0f; // Coefficient 'c'
    [SerializeField] private int nrOfPoints = 100; // Number of points to draw
    public FunctionType functionType = FunctionType.Linear; // Type of function to draw
    public string function;
    public Transform ball;
    private FollowCurve followCurve;

    public void DrawFunction()
    {
        if (ball == null) return;
        switch (functionType)
        {
            case FunctionType.Linear:
                function = "y = " + a + "x + " + b;
                DrawLinearFunction();
                break;
            case FunctionType.Exponential:
                function = "y = " + a + "x^2 + " + b + "x + " + c;
                DrawExponentialFunction();
                break;
        }
    }

    private void DrawLinearFunction()
    {
        LineRenderer lineRenderer = GetOrCreateLineRenderer();
        lineRenderer.positionCount = nrOfPoints;
        followCurve = ball.GetComponent<FollowCurve>();

        Vector3[] points = new Vector3[nrOfPoints];
        for (int i = 0; i < nrOfPoints; i++)
        {
            float x = i * (10f / (nrOfPoints - 1));
            float z = a * x + b;
            points[i] = new Vector3(x, 0, z);
        }

        // Offset curve so it starts at balls position
        Vector3 offset = ball.position - points[0];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += offset;
        }

        lineRenderer.SetPositions(points);
        followCurve.curvePoints = points;
    }

    private void DrawExponentialFunction()
    {
        LineRenderer lineRenderer = GetOrCreateLineRenderer();
        lineRenderer.positionCount = nrOfPoints;
        followCurve = ball.GetComponent<FollowCurve>();

        Vector3[] points = new Vector3[nrOfPoints];

        for (int i = 0; i < nrOfPoints; i++)
        {
            float x = i * (10f / (nrOfPoints - 1)); // Spread along X-axis
            float z = a * Mathf.Pow(x, 2) + b * x + c; // Exponential (quadratic) function
            float scaledCurve = z * 0.1f; //scale curve down to retain shape

            points[i] = ball.position + new Vector3(x, 0, scaledCurve);
        }

        lineRenderer.SetPositions(points);
        followCurve.curvePoints = points;
    }

    public LineRenderer GetOrCreateLineRenderer()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // Clear existing points
        return lineRenderer;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(FunctionDrawer))]
public class FunctionDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FunctionDrawer functionDrawer = (FunctionDrawer)target;
        if (GUILayout.Button("Update Function"))
        {
            functionDrawer.DrawFunction();
        }
    }
}
#endif

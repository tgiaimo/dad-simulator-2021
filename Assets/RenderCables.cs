using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCables : MonoBehaviour
{
    private LineRenderer thisRenderer;
    public GameObject pos1;
    public GameObject pos2;
    public GameObject pos3;
    public float curve = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        pos2.transform.position = (pos1.transform.position + pos3.transform.position) / 2;
        // float curve_transformed = (1 / pos2.transform.position.magnitude) * curve;
        pos2.transform.position += new Vector3(0, -curve, 0);
        DrawQuadraticBezierCurve(pos1.transform.position, pos2.transform.position, pos3.transform.position);
    }

    // https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/
    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        thisRenderer.positionCount = 200;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < thisRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            thisRenderer.SetPosition(i, B);
            t += (1 / (float)thisRenderer.positionCount);
        }
    }
}

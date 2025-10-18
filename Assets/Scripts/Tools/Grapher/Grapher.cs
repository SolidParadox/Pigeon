using System;
using UnityEngine;

public class Grapher : MonoBehaviour {
    public LineRenderer lineRenderer;

    public LineRenderer lineRendererBaseline;
    public LineRenderer lineRendererTopline;

    public Vector2 dataScale;
    public Vector3 framePosition;
    public int dataPoints;
    protected int index;

    public float topLineValue;

    protected float [] data;

    void Start () {
        data = new float [ dataPoints ];
        lineRenderer.positionCount = dataPoints;

        lineRendererBaseline.positionCount = 2;
        lineRendererTopline.positionCount = 2;
    }

    public Vector2 GetPoint ( int index , int offset ) {
        return Vector2.Scale ( new Vector2 ( offset , data [ index ] ) , dataScale );
    }

    public virtual void Update () {
        Vector2 delta;
        for ( int i = 0; i < dataPoints; i++ ) {
            delta = GetPoint ( ( index + i - 1 + dataPoints ) % dataPoints , i );
            lineRenderer.SetPosition ( i , transform.TransformPoint ( ( Vector3 ) delta + framePosition ) );
        }
        index = ( index + 1 ) % dataPoints;

        lineRendererBaseline.SetPosition ( 0 , transform.TransformPoint ( framePosition ) );
        lineRendererBaseline.SetPosition ( 1 , transform.TransformPoint ( new Vector3 ( dataPoints * dataScale.x , 0 ) + framePosition ) );

        lineRendererTopline.SetPosition ( 0 , transform.TransformPoint ( Vector3.Scale ( new Vector3 ( 0, topLineValue ) , dataScale ) + framePosition ) );
        lineRendererTopline.SetPosition ( 1 , transform.TransformPoint ( Vector3.Scale ( new Vector3 ( dataPoints , topLineValue ) , dataScale ) + framePosition ) );
    }
}

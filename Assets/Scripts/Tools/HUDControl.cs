using UnityEngine;

public class HUDControl : MonoBehaviour {
    public FlightDriver FlightDriver;

    public LineRenderer LineRenderer;
    public LineRenderer LineRendererCircle;

    private int circleLength = 16;

    private void Start () {
        LineRenderer.positionCount = 2;
        LineRendererCircle.positionCount = circleLength;
    }

    void Update () {
        for ( int i = 0; i < circleLength; i++ ) {
            LineRendererCircle.SetPosition ( i , transform.TransformPoint ( Quaternion.Euler ( 0 , 0 , ( i + 1 ) * 360 / circleLength ) * Vector3.up ) );
        }
        LineRenderer.SetPosition ( 0 , transform.TransformPoint ( Vector3.zero ) );
        LineRenderer.SetPosition ( 1, transform.TransformPoint ( FlightDriver.currentInput / FlightDriver.maxInput ) );
    }
}

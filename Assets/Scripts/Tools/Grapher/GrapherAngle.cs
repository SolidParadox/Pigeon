using UnityEngine;

public class GrapherAngle : Grapher {
    public FlightCore FlightCore;
    public enum Axis { X, Z };
    public Axis axis;

    public float delta;

    public bool clamped;

    public override void Update () {
        // TODO : Fix this with new flightcore
        if ( clamped ) { delta = Mathf.RoundToInt ( delta ) % 360; }

        data [ index ] = delta;
        base.Update ();
    }
}

using UnityEngine;

public class GrapherAngle : Grapher {
    public FlightCore FlightCore;
    public enum Axis { X, Z };
    public Axis axis;

    public float delta;

    public bool clamped;

    public override void Update () {
        delta = FlightCore.pastX;
        if ( axis == Axis.Z ) {
            delta = FlightCore.pastZ;
        }
        if ( clamped ) { delta = Mathf.RoundToInt ( delta ) % 360; }

        data [ index ] = delta;
        base.Update ();
    }
}

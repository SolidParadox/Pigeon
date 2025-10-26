using UnityEngine;

public class GrapherAngle : Grapher {
    public FlightCore FlightCore;
    public enum Axis { X, Z };
    public Axis axis;

    public float delta;
    public float pastDelta;
    public int whatever;

    public override void Update () {
        delta = FlightCore.pastX;
        if ( axis == Axis.Z ) {
            delta = FlightCore.pastZ;
        }

        data [ index ] = delta;
        pastDelta = delta;
        base.Update ();
    }
}

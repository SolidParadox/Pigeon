using UnityEngine;

public class GrapherAngle : Grapher {
    public Rigidbody rgb;
    public enum Axis { X, Z };
    public Axis axis;

    public float delta;
    public float pastDelta;
    public int whatever;

    public override void Update () {
        Vector3 markRoll = Vector3.ProjectOnPlane( Vector3.up, rgb.transform.forward );
        if ( Vector3.up == rgb.transform.forward ) {
            markRoll = rgb.transform.up;
        }
        Vector3 markPitch = rgb.transform.forward;
        if ( rgb.linearVelocity.sqrMagnitude > 0 ) {
            markPitch = Vector3.ProjectOnPlane ( rgb.transform.InverseTransformDirection ( rgb.linearVelocity.normalized ) , rgb.transform.right );
        }

        float pitchDelta    = Vector3.SignedAngle ( rgb.transform.forward , markPitch , rgb.transform.right );
        float rollDelta     = Vector3.SignedAngle ( rgb.transform.up , markRoll , rgb.transform.forward );

        delta = pitchDelta;
        if ( axis == Axis.Z ) {
            delta = rollDelta;
            Debug.Log ( delta + " " + pastDelta );
        }

        delta += 360 * whatever;

        if ( Mathf.Abs( delta - pastDelta ) > 180 ) {
            delta -= 360 * whatever;
            whatever += ( delta + 360 * whatever ) < pastDelta ? 1 : -1;
            delta += 360 * whatever;
            Debug.Break ();
        }

        data [ index ] = delta;
        pastDelta = delta;
        base.Update ();
    }
}

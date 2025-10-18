using UnityEngine;

public class Anchor : MonoBehaviour {
    public Transform anchor;

    public float STR_linearInterpolation = 25f;
    public Vector3 BIAS_linearInterpolation = Vector3.one;

    public float STR_angularInterpolation = 25f;

    public Vector3 linearInterpolationOffset;
    public Vector3 angularInterpolationOffset;

    public bool SWITCH_LOR; // Linear offset is relative

    void LinearInterpolation ( float t ) {
        Vector3 target = anchor.position;
        if ( SWITCH_LOR ) {
            Quaternion angularRot = Quaternion.Euler(angularInterpolationOffset.x, angularInterpolationOffset.y, 0);
            target = anchor.position + ( anchor.rotation * ( angularRot * linearInterpolationOffset ) );
        } else {
            target = anchor.position + linearInterpolationOffset;
        }
        Vector3 delta = target - transform.position;
        delta.Scale ( BIAS_linearInterpolation );
        transform.position += delta * STR_linearInterpolation * t;
    }

    void AngularInterpolation ( float t ) {
        Quaternion targetRotation = anchor.rotation * Quaternion.Euler(angularInterpolationOffset);
        transform.rotation = Quaternion.Slerp (
            transform.rotation ,
            targetRotation ,
            STR_angularInterpolation * t
        );
    }

    private void LateUpdate () {
        LinearInterpolation ( Time.deltaTime );
        AngularInterpolation ( Time.deltaTime );
    }
}

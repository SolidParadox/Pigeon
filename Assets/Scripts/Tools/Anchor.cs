using UnityEngine;

public class Anchor : MonoBehaviour {
    public Transform anchor;

    public float        STR_linearInterpolation = 25;
    public Vector3      BIAS_linearInterpolation = Vector3.one;

    public float        STR_angularInterpolation = 25;

    public Vector3      linearInterpolationOffset;
    public Vector3      angularInterpolationOffset;

    public bool         SWITCH_DT;      // Delta Time ( default Fixed Delta time )
    public bool         SWITCH_RT;      // Real Time

    private int         timebit = 0;
    private void Start () {
        timebit = SWITCH_DT ? 1 : 0 + ( SWITCH_RT ? 2 : 0 );
    }

    void LinearInterpolation ( float t ) {
        Vector3 delta = anchor.position - linearInterpolationOffset - transform.position;
        delta.Scale ( BIAS_linearInterpolation );
        delta = Vector3.LerpUnclamped ( Vector3.zero , -delta , t * STR_linearInterpolation );
        delta.Scale ( BIAS_linearInterpolation );
        transform.Translate ( -delta );
    }

    void AngularInterpolation ( float t ) {                          
        Quaternion offsetRotation = Quaternion.Euler(anchor.eulerAngles + angularInterpolationOffset);
        Quaternion delta = offsetRotation * Quaternion.Inverse(transform.rotation);
        Quaternion interpolatedDelta = Quaternion.SlerpUnclamped(Quaternion.identity, delta, t * STR_angularInterpolation);
        transform.rotation = interpolatedDelta * transform.rotation;
    }

    private void Update () {
        LinearInterpolation ( Time.deltaTime );
        AngularInterpolation ( Time.deltaTime );
    }
}
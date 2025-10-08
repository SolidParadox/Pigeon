using UnityEngine;

public class Anchor : MonoBehaviour {
    public Transform anchor;

    public float STR_linearInterpolation = 25f;
    public Vector3 BIAS_linearInterpolation = Vector3.one;

    public float STR_angularInterpolation = 25f;

    public Vector3 linearInterpolationOffset;
    public Vector3 angularInterpolationOffset;

    public bool SWITCH_DT;  // use deltaTime
    public bool SWITCH_RT;  // use realTimeSinceStartup

    private float lastTime;

    private void Start () {
        lastTime = Time.realtimeSinceStartup;
    }

    void LinearInterpolation ( float t ) {
        Vector3 target = anchor.position + linearInterpolationOffset;
        Vector3 delta = target - transform.position;
        delta.Scale ( BIAS_linearInterpolation );
        transform.position += delta * STR_linearInterpolation * t;
    }

    void AngularInterpolation ( float t ) {
        Quaternion targetRotation = Quaternion.Euler(anchor.eulerAngles + angularInterpolationOffset);
        transform.rotation = Quaternion.Slerp ( transform.rotation , targetRotation , STR_angularInterpolation * t );
    }

    private void Update () {
        float dt = SWITCH_RT ? Time.realtimeSinceStartup - lastTime :
                  (SWITCH_DT ? Time.deltaTime : Time.fixedDeltaTime);
        lastTime = Time.realtimeSinceStartup;

        LinearInterpolation ( dt );
        AngularInterpolation ( dt );
    }
}

using UnityEngine;

public class FlightCore : MonoBehaviour {
    public Rigidbody    rgb;

    public Vector3 STRMultipliers;
    public float STRMultipliersVertical;
    
    public float STRExpoDrag;

    public float THRSpeed;
    public float THRSpeedVertical;
    public AnimationCurve FNCAcc;

    private int   passPrio = 0;
    private Vector3 mainPassVar;
    private float verticalPassVar;

    private void Start () {
        mainPassVar = Vector3.zero;
        verticalPassVar = 0;
    }

    public void RX( Vector3 alphaCombined, float alphaVertical, int priority = 0 ) {
        if ( priority >= passPrio ) {
            mainPassVar = alphaCombined;
            verticalPassVar = alphaVertical;
        }
    }

    public void Boost () {
        mainPassVar.z = 1;
    }

    private void FixedUpdate () {
        mainPassVar.Scale ( STRMultipliers );

        rgb.MoveRotation ( rgb.rotation * Quaternion.Euler ( 0 , mainPassVar.y , 0 ) );

        Vector3 xyVel = rgb.linearVelocity;
        xyVel -= xyVel.normalized * xyVel.sqrMagnitude * STRExpoDrag;
        rgb.linearVelocity = xyVel;

        xyVel = rgb.transform .InverseTransformDirection ( xyVel );
        Vector3 xyAcc = new Vector3 ( mainPassVar.x, 0, mainPassVar.z ) * Time.fixedDeltaTime;

        Vector3 accumulator = Vector3.zero;

        accumulator += MIKA ( new Vector3 ( 0 , verticalPassVar * STRMultipliersVertical * Time.fixedDeltaTime , 0 ) , new Vector3 ( 0, xyVel.y, 0 ) , THRSpeedVertical );

        xyVel.y = 0;

        xyAcc.x *= FNCAcc.Evaluate ( xyVel.x / THRSpeed );
        xyAcc.z *= FNCAcc.Evaluate ( xyVel.z / THRSpeed );

        accumulator += MIKA ( xyAcc , xyVel , THRSpeed );
        rgb.AddRelativeForce ( accumulator , ForceMode.VelocityChange );

        mainPassVar = Vector3.zero;
        passPrio = 0;
    }


    // The holy relic
    public static Vector3 MIKA ( Vector3 a , Vector3 b , float m ) {
        if ( a.magnitude < 0.0001f ) return Vector2.zero;
        if ( ( a + b ).magnitude >= m ) {
            if ( Vector3.Dot ( a , b ) < 0 ) { return a.normalized * Mathf.Min ( a.magnitude , m ); }
            if ( b.magnitude > m ) { b = b.normalized * m; }
            return ( b + a ).normalized * m - b;
        }
        return a;
    }
}

using UnityEngine;

public class FlightCore : MonoBehaviour {
    public Rigidbody    rgb;

    public Vector3 STRMultipliers;
    public float STRMultipliersVertical;
    public float THRSpeed;
    public float THRSpeedVertical;
    public AnimationCurve FNCAcc;

    private Vector3 transfers;
    private float transfersVertical;

    private void Start () {
        transfers = Vector3.zero;
    }

    public void RXinput( Vector3 alphaCombined, float alphaVertical ) {
        transfers = alphaCombined;
        transfersVertical = alphaVertical;  
    }

    private void FixedUpdate () {
        transfers.Scale ( STRMultipliers );

        rgb.MoveRotation ( rgb.rotation * Quaternion.Euler ( 0 , transfers.y , 0 ) );

        Vector3 xyVel = rgb.transform.InverseTransformDirection ( rgb.linearVelocity );
        Vector3 xyAcc = new Vector3 ( transfers.x, 0, transfers.z ) * Time.fixedDeltaTime;

        Vector3 accumulator = Vector3.zero;

        accumulator += MIKA ( new Vector3 ( 0 , transfersVertical * STRMultipliersVertical * Time.fixedDeltaTime , 0 ) , new Vector3 ( 0, xyVel.y, 0 ) , THRSpeedVertical );

        xyVel.y = 0;

        xyAcc.x *= FNCAcc.Evaluate ( xyVel.x / THRSpeed );
        xyAcc.z *= FNCAcc.Evaluate ( xyVel.z / THRSpeed );

        accumulator += MIKA ( xyAcc , xyVel , THRSpeed );
        rgb.AddRelativeForce ( accumulator , ForceMode.VelocityChange );

        transfers = Vector3.zero;
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

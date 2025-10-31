using UnityEngine;

public class FlightCore : MonoBehaviour {
    public Rigidbody    rgb;

    public Vector2 alignmentPower;

    public Vector2  controlsSensitivity;

    // Controls control variables ( lel )
    private Vector2 prTheta;

    // Wings control variables
    private bool    wingTheta;
    private bool wingThetaCatch;

    private int      deltaStatus;
    public float    STRredirect;

    public float    STRwingflap;
    public float    THRwingZtoXY;

    public float    STRexpoDrag;

    public LineRenderer debugLineRenderer;

    public PID controllerRoll, controllerPitch;

    private int windupX = 0, windupZ = 0;
    public float pastX = 0, pastZ = 0;

    [SerializeField]
    private AnimationCurve STRspeedWingRedirect = AnimationCurve.Linear(0, 0, 1, 1);
    public void UpdateInputs ( Vector2 pitchRoll , bool wings ) {
        if ( deltaStatus < 2 ) {
            prTheta = pitchRoll;
        } else {
            prTheta = ( prTheta + pitchRoll ) / 2;
        }
        if ( !wingTheta && wings ) {
            wingThetaCatch = true;
        }
        wingTheta = wings | wingThetaCatch;
        deltaStatus = 2;
    }

    void FixedUpdate () {
        if ( deltaStatus <= 0 ) {
            wingTheta = false;
            prTheta = Vector2.zero;
        }

        prTheta.Scale ( controlsSensitivity );
        rgb.AddRelativeTorque ( prTheta.y , 0 , prTheta.x , ForceMode.Force );

        if ( wingTheta && wingThetaCatch ) {
            Vector2 horizontalSpeed = rgb.linearVelocity;
            horizontalSpeed.y = 0;
            Vector2 targetForce = Vector3.Lerp ( Vector3.up, rgb.transform.forward, horizontalSpeed.magnitude / THRwingZtoXY );
            rgb.AddForce ( targetForce * STRwingflap , ForceMode.Impulse );
            wingThetaCatch = false;
        }

        Vector3 rv = rgb.linearVelocity;
        Vector3 tv = rv.magnitude * rgb.transform.forward;

        rgb.AddForce ( ( tv - rv ) * STRspeedWingRedirect.Evaluate ( rv.magnitude ) * STRredirect * Time.fixedDeltaTime , ForceMode.VelocityChange );

        /*╔═══════════════════════════════════════════════════════════════╗
          ║                       Righting mechanism                      ║
          ╚═══════════════════════════════════════════════════════════════╝*/

        debugLineRenderer.SetPosition ( 0 , rgb.transform.position );
        debugLineRenderer.SetPosition ( 1 , rgb.transform.position + rgb.linearVelocity * 10 );

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

        pitchDelta += 360 * windupX;
        if ( Mathf.Abs ( pitchDelta - pastX ) > 180 ) {
            pitchDelta -= 360 * windupX;
            windupX += ( pitchDelta + 360 * windupX ) < pastX ? 1 : -1;
            pitchDelta += 360 * windupX;
        }

        rollDelta += 360 * windupZ;
        if ( Mathf.Abs ( rollDelta - pastZ ) > 180 ) {
            rollDelta -= 360 * windupZ;
            windupZ += ( rollDelta + 360 * windupZ ) < pastZ ? 1 : -1;
            rollDelta += 360 * windupZ;
        }

        float pitchResult = controllerPitch.Compute ( 360 * windupX , pitchDelta );
        float rollResult = controllerRoll.Compute ( 360 * windupZ, rollDelta );

        float gimbalLockAngle = Vector3.Angle ( rgb.transform.forward , Vector3.up );

        if ( gimbalLockAngle < 10 || gimbalLockAngle > 170 ) {
            rollResult = 0;
            controllerRoll.Reset ();
            Debug.Log ( "RESET!" );
        }

        pastX = pitchDelta;
        pastZ = rollDelta;

        //Debug.Log( rgb.angularVelocity + " " + rgb.transform.TransformDirection ( rgb.angularVelocity ) + " " + rollDelta + " " + rollResult);

        rgb.AddRelativeTorque ( pitchResult * alignmentPower.y , 0 , rollResult * alignmentPower.x , ForceMode.VelocityChange );

        rv = rgb.linearVelocity;
        rv = rv.normalized * ( rv.magnitude * rv.magnitude ) * STRexpoDrag;
        rgb.AddForce ( -rv , ForceMode.VelocityChange );

        deltaStatus--;
    }
}

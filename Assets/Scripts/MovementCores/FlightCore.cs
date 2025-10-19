using UnityEngine;

public class FlightCore : MonoBehaviour {
    public Vector2      target;
    public Rigidbody    rgb;

    public float        wingsStatus;

    [System.Serializable]
    public class FlightCharacteristics {
        public float glideRatio;
        public float bankRatio;
        public float diveRatio;
        public float expoDrag;

        public float wingsDuration;
        public float wingsPower;

        public Vector2 prVelocityRatio;
    }

    public Vector2 alignmentPower;
    public FlightCharacteristics characteristics;

    public Vector2 prTheta;
    public bool    wingTheta;

    public int deltaStatus;

    public LineRenderer debugLineRenderer;

    public PID controllerRoll, controllerPitch;

    public void UpdateInputs( Vector2 pitchRoll, bool wings ) {
        if ( deltaStatus < 2 ) {
            prTheta = pitchRoll;
        } else {
            prTheta = ( prTheta + pitchRoll ) / 2;
        }
        wingTheta |= wings;
        deltaStatus = 2;                        
    }

    void FixedUpdate () {
        if ( deltaStatus <= 0 ) {
            wingTheta = false;
            prTheta = Vector2.zero;
        }

        prTheta.Scale ( characteristics.prVelocityRatio );
        rgb.AddRelativeTorque ( prTheta.y , 0 , prTheta.x , ForceMode.Force );

        if ( Vector3.Dot ( rgb.transform.forward, Vector3.up ) > 0 ) {
            
        }

        if ( wingTheta ) {
            rgb.AddForce ( rgb.transform.forward * characteristics.wingsPower );
        }

        Vector3 rv = rgb.transform.InverseTransformDirection( rgb.linearVelocity );

        rv.y = ( Mathf.Abs( rv.z ) - Mathf.Abs( rv.y ) ) * characteristics.glideRatio;
        if ( rv.y > 0 && rv.z > 0 ) {
            rv.z = 0;
            rv.x = 0;
            rv.y *= rv.y;
            rgb.AddForce ( rgb.transform.TransformDirection ( rv ) , ForceMode.VelocityChange );
        }

        /*?????????????????????????????????????????????????????????????????
          ?                       Righting mechanism                      ?
          ?????????????????????????????????????????????????????????????????*/

        debugLineRenderer.SetPosition ( 0 , rgb.transform.position );
        debugLineRenderer.SetPosition ( 1 , rgb.transform.position + rgb.linearVelocity * 10 );

        Vector3 markRoll = Vector3.ProjectOnPlane( Vector3.up, rgb.transform.forward ); 
        if ( Vector3.up == rgb.transform.forward ) {
            markRoll = rgb.transform.up;
        }
        Vector3 markPitch = rgb.transform.forward;
        if ( rgb.linearVelocity.sqrMagnitude > 0 ) {
            markPitch = Vector3.ProjectOnPlane ( rgb.transform.InverseTransformDirection ( rgb.linearVelocity.normalized ), rgb.transform.right );
        }

        float pitchDelta    = Vector3.SignedAngle ( rgb.transform.forward , markPitch , rgb.transform.right );
        float rollDelta     = Vector3.SignedAngle ( rgb.transform.up , markRoll , rgb.transform.forward );

        float pitchResult = controllerPitch.Compute ( 0 , pitchDelta );
        float rollResult = controllerRoll.Compute ( 0, rollDelta );

        //Debug.Log( rgb.angularVelocity + " " + rgb.transform.TransformDirection ( rgb.angularVelocity ) + " " + rollDelta + " " + rollResult);

        rgb.AddRelativeTorque ( pitchResult *  alignmentPower.x, 0, rollResult * alignmentPower.y, ForceMode.VelocityChange );

        rv = rgb.linearVelocity;
        rv = rv.normalized * ( rv.magnitude * rv.magnitude ) * characteristics.expoDrag;
        rgb.AddForce ( -rv, ForceMode.VelocityChange );

        deltaStatus--;
    }
}

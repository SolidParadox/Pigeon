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

        if ( wingTheta ) {
            // If grounded, should do lift off 
            rgb.AddForce ( rgb.transform.forward * characteristics.wingsPower );
        }

        Vector3 rv = rgb.transform.InverseTransformDirection( rgb.linearVelocity );

        //Debug.Log ( rv );

        rv.y = ( Mathf.Abs( rv.z ) - Mathf.Abs( rv.y ) ) * characteristics.glideRatio;
        if ( rv.y > 0 && rv.z > 0 ) {
            rv.z = 0;
            rv.x = 0;
            rv.y *= rv.y;
            rgb.AddForce ( rgb.transform.TransformDirection ( rv ) , ForceMode.VelocityChange );
        }

        // Righting mechanism 
        debugLineRenderer.SetPosition ( 0 , rgb.transform.position );
        debugLineRenderer.SetPosition ( 1 , rgb.transform.position + rgb.linearVelocity * 10 );

        float pitchDelta    = Vector3.SignedAngle ( rgb.transform.forward , rgb.linearVelocity , rgb.transform.right );
        float rollDelta     = Vector3.SignedAngle ( rgb.transform.up , Vector3.up , rgb.transform.forward );

        rgb.AddRelativeTorque ( pitchDelta *  alignmentPower.x, 0 , rollDelta * alignmentPower.y , ForceMode.VelocityChange );

        rv = rgb.linearVelocity;
        rv = rv.normalized * ( rv.magnitude * rv.magnitude ) * characteristics.expoDrag * Time.deltaTime;
        rgb.AddForce ( -rv, ForceMode.VelocityChange );

        deltaStatus--;
    }
}

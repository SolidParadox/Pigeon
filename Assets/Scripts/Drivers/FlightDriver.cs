using UnityEngine;
using UnityEngine.InputSystem;

public class FlightDriver : MonoBehaviour {
    public FlightCore FlightCore;
    public InputActionAsset InputActions;
    public Anchor CameraAnchor;

    private InputAction lookAction;
    private InputAction wingsAction;
    private InputAction releaseControl;
    private InputAction releaseCamera;

    public bool inControl;
    public bool cameraControl;

    public Vector2 cameraSensitivityMultiplier;

    private void OnEnable () {
        inControl = true;
        cameraControl = false;

        var map = InputActions.FindActionMap("PigeonFlight");
        map.Enable ();

        wingsAction     = map.FindAction ( "PigeonWingFlap" );
        lookAction      = map.FindAction ( "Look" );
        releaseControl  = map.FindAction ( "ReleaseControl" );
        releaseCamera   = map.FindAction ( "ReleaseCamera" );
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "PigeonFlight" ).Disable ();
    }

    void Update () {
        if ( releaseControl.WasPressedThisDynamicUpdate () ) {
            inControl = !inControl;
        }
        
        if ( !inControl ) return;
        
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * Time.deltaTime;

        if ( releaseCamera.WasPerformedThisDynamicUpdate () ) {
            cameraControl = ! cameraControl;
            if ( !cameraControl ) {
                CameraAnchor.angularInterpolationOffset = Vector3.zero;
            }
        }

        if ( cameraControl ) {
            lookDelta.Scale(cameraSensitivityMultiplier );
            CameraAnchor.angularInterpolationOffset += new Vector3 ( -lookDelta.y, lookDelta.x, 0);
            lookDelta = Vector2.zero;
        }

        FlightCore.UpdateInputs ( lookDelta , wingsAction.IsPressed () );
    }
}

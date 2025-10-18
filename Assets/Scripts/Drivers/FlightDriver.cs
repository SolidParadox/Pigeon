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
    private Vector2 savedFlightInputs;

    private void OnEnable () {
        inControl = true;
        cameraControl = false;

        var map = InputActions.FindActionMap("PigeonFlight");
        map.Enable ();

        wingsAction     = map.FindAction ( "PigeonWingFlap" );
        lookAction      = map.FindAction ( "Look" );
        releaseControl  = map.FindAction ( "ReleaseControl" );
        releaseCamera   = map.FindAction ( "ReleaseCamera" );

        Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "PigeonFlight" ).Disable ();
    }

    void Update () {
        if ( releaseControl.WasPressedThisDynamicUpdate () ) {
            inControl = !inControl;
            Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        if ( !inControl ) return;
        
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * Time.deltaTime;

        if ( cameraControl != releaseCamera.IsPressed() ) {
            cameraControl = releaseCamera.IsPressed ();
            if ( !cameraControl ) {
                CameraAnchor.angularInterpolationOffset = Vector3.zero;
            } else {
                savedFlightInputs = lookDelta;
            }
        }

        if ( cameraControl ) {
            lookDelta.Scale(cameraSensitivityMultiplier );
            CameraAnchor.angularInterpolationOffset += new Vector3 ( -lookDelta.y, lookDelta.x, 0);
            lookDelta = savedFlightInputs;
        }

        FlightCore.UpdateInputs ( lookDelta , wingsAction.IsPressed () );
    }
}

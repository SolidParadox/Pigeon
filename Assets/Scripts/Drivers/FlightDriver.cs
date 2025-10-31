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
    private InputAction resetPigeon;

    public bool inControl;
    public bool cameraControl;

    public Vector2 cameraSensitivityMultiplier;

    public Vector2 currentInput;
    public float maxInput;

    private void OnEnable () {
        inControl = true;
        cameraControl = false;
        currentInput = Vector2.zero;

        var map = InputActions.FindActionMap("PigeonFlight");
        map.Enable ();

        wingsAction     = map.FindAction ( "PigeonWingFlap" );
        lookAction      = map.FindAction ( "Look" );
        releaseControl  = map.FindAction ( "ReleaseControl" );
        releaseCamera   = map.FindAction ( "ReleaseCamera" );
        resetPigeon     = map.FindAction ( "ResetPigeon" );

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

        if ( resetPigeon.WasPressedThisDynamicUpdate () ) {
            FlightCore.rgb.linearVelocity = Vector3.zero;
            FlightCore.rgb.angularVelocity = Vector3.zero;
            FlightCore.rgb.transform.position = Vector3.zero;
            FlightCore.rgb.transform.rotation = Quaternion.identity;
        }

        if ( !inControl ) return;
        
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * Time.deltaTime;

        if ( cameraControl != releaseCamera.IsPressed() ) {
            cameraControl = releaseCamera.IsPressed ();
            if ( !cameraControl ) {
                CameraAnchor.angularInterpolationOffset = Vector3.zero;
            }
        }

        if ( cameraControl ) {
            lookDelta.Scale(cameraSensitivityMultiplier );
            CameraAnchor.angularInterpolationOffset += new Vector3 ( -lookDelta.y, lookDelta.x, 0);
            lookDelta = Vector2.zero;
        }
        currentInput += lookDelta; 
        if ( currentInput.magnitude > maxInput ) {
            currentInput = currentInput.normalized * maxInput;
        }

        FlightCore.UpdateInputs ( Quaternion.Euler( 0, 0, -CameraAnchor.transform.localRotation.eulerAngles.z ) * ( currentInput.normalized * currentInput.sqrMagnitude / maxInput ) , wingsAction.IsPressed () );
    }
}

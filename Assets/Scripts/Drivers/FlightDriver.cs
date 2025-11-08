using UnityEngine;
using UnityEngine.InputSystem;

public class FlightDriver : MonoBehaviour {
    public FlightCore FlightCore;
    public InputActionAsset InputActions;
    public Anchor CameraAnchor;

    private InputAction rightStickAction;
    private InputAction leftStickAction;

    private InputAction verticalAction;

    private InputAction releaseControl;
    private InputAction releaseCamera;

    public bool inControl;
    public bool cameraControl;

    public Vector2 cameraSensitivityMultiplier;

    public Vector2  transferLeft;
    public Vector2  transferRight;
    public float    transferVertical;
    private int ticker;

    ///  RIGHT --- IS ---> VIEW
    ///  LEFT  --- IS --->  XY

    private void OnEnable () {
        inControl = true;

        var map = InputActions.FindActionMap("Player");
        map.Enable ();

        verticalAction = map.FindAction ( "Vertical" );
        rightStickAction = map.FindAction ( "Look" );
        leftStickAction = map.FindAction ( "Move" );
        releaseControl = map.FindAction ( "TEMPReleaseControl" );

        Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;

        ticker = 0; transferRight = Vector2.zero; transferLeft = Vector2.zero;
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "Player" ).Disable ();
    }

    void Update () {
        if ( releaseControl.WasPressedThisDynamicUpdate () ) {
            inControl = !inControl;
            Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if ( !inControl ) return;

        if ( ticker == 0 ) {
            transferLeft = Vector2.zero;
            transferRight = Vector2.zero;
            ticker = 0;
        }

        Vector2 delta = leftStickAction.ReadValue<Vector2> ();
        if ( delta.magnitude > 1 ) { delta.Normalize (); }

        transferLeft += delta;

        // WARNING : INPUT GETS OVERWHELMED WHEN USING MOUSE, should clamp at a different point for mouse only - fix later
        delta = rightStickAction.ReadValue<Vector2> ();
        if ( delta.magnitude > 1 ) { delta.Normalize (); }

        transferRight += delta;

        ticker++;

        transferVertical += verticalAction.ReadValue<float> ();

        /*
        if ( cameraControl != releaseCamera.IsPressed() ) {
            cameraControl = releaseCamera.IsPressed ();
            if ( !cameraControl ) {
                CameraAnchor.angularInterpolationOffset = Vector3.zero;
            }
        }
        */
    }

    private void FixedUpdate () {
        Vector2 deltaRight = transferRight, deltaLeft = transferLeft;
        if ( ticker > 1 ) { deltaRight /= ticker; deltaLeft /= ticker; transferVertical /= ticker; }

        // Anti lag system for dropped update frames
        // Send information to the damn Core
        FlightCore.RXinput ( new Vector3 ( deltaLeft.x , deltaRight.x , deltaLeft.y ) , transferVertical );
        ticker = 0;
    }
}

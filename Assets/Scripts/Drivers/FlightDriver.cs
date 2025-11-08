using UnityEngine;
using UnityEngine.InputSystem;

public struct FloatTransfer {
    private float accumulator;
    public void Add ( float input ) => accumulator += input;
    public void TryReset ( int ticker ) {
        if ( ticker == 0 ) accumulator = 0f;
    }
    public float GetValue ( int ticker ) => ticker > 0 ? accumulator / ticker : accumulator;
}
public struct Vector2Transfer {
    private Vector2 accumulator;
    public void Add ( Vector2 input ) => accumulator += input;
    public void TryReset ( int ticker ) {
        if ( ticker == 0 ) accumulator = Vector2.zero;
    }
    public Vector2 GetValue ( int ticker ) => ticker > 0 ? accumulator / ticker : accumulator;
}

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

    private Vector2Transfer transferLeft;
    private Vector2Transfer transferRight;
    private FloatTransfer transferVertical;

    private int ticker;

    private void OnEnable () {
        inControl = true;

        var map = InputActions.FindActionMap("Player");
        map.Enable ();

        verticalAction = map.FindAction ( "Vertical" );
        rightStickAction = map.FindAction ( "Look" );
        leftStickAction = map.FindAction ( "Move" );
        releaseControl = map.FindAction ( "TEMPReleaseControl" );

        Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;

        ticker = 0;
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

        transferLeft.TryReset ( ticker );
        transferRight.TryReset ( ticker );
        transferVertical.TryReset ( ticker );

        Vector2 delta = leftStickAction.ReadValue<Vector2>();
        if ( delta.magnitude > 1 ) delta.Normalize ();
        transferLeft.Add ( delta );

        delta = rightStickAction.ReadValue<Vector2> ();
        if ( delta.magnitude > 1 ) delta.Normalize ();
        transferRight.Add ( delta );

        transferVertical.Add ( verticalAction.ReadValue<float> () );

        ticker++;
    }

    private void FixedUpdate () {
        Vector2 deltaRight = transferRight.GetValue(ticker);
        Vector2 deltaLeft = transferLeft.GetValue(ticker);
        float deltaVertical = transferVertical.GetValue(ticker);

        FlightCore.RXinput ( new Vector3 ( deltaLeft.x , deltaRight.x , deltaLeft.y ) , deltaVertical );

        ticker = 0;
    }
}
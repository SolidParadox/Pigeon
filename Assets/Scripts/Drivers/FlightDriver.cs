using UnityEngine;
using UnityEngine.InputSystem;

public class FlightDriver : MonoBehaviour {
    public FlightCore FlightCore;
    public InputActionAsset InputActions;

    private InputAction lookAction;
    private InputAction wingsAction;
    private InputAction releaseControl;

    public bool inControl;

    private void OnEnable () {
        inControl = true;
        var map = InputActions.FindActionMap("PigeonFlight");
        map.Enable ();

        wingsAction = map.FindAction ( "PigeonWingFlap" );
        lookAction = map.FindAction ( "Look" );
        releaseControl = map.FindAction ( "ReleaseControl" );
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "PigeonFlight" ).Disable ();
    }

    void Update () {
        if ( releaseControl.WasPressedThisDynamicUpdate () ) {
            inControl = !inControl;
        }
        
        if ( !inControl ) return;
        
        Vector2 lookDelta = lookAction.ReadValue<Vector2>();
        float mouseX = lookDelta.x;
        float mouseY = lookDelta.y;

        FlightCore.UpdateInputs ( lookDelta , wingsAction.IsPressed () );
    }
}

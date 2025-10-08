using UnityEngine;
using UnityEngine.InputSystem;

public class FlightDriver : MonoBehaviour {
    public FlightCore FlightCore;
    public InputActionAsset InputActions;

    private InputAction lookAction;
    private InputAction wingsAction;

    private void OnEnable () {
        var map = InputActions.FindActionMap("PigeonFlight");
        map.Enable ();

        wingsAction = map.FindAction ( "PigeonWingFlap" );
        lookAction = map.FindAction ( "Look" );
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "PigeonFlight" ).Disable ();
    }

    void Update () {
        Vector2 lookDelta = lookAction.ReadValue<Vector2>();
        float mouseX = lookDelta.x;
        float mouseY = lookDelta.y;

        FlightCore.UpdateInputs ( lookDelta , wingsAction.IsPressed () );
    }
}

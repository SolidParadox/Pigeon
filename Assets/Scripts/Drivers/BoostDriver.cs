using UnityEngine;
using UnityEngine.InputSystem;

public class BoostDriver : MonoBehaviour {
    public FlightDriver FlightDriver;
    public FlightCore FlightCoreNormal;
    public FlightCore FlightCoreBoost;

    public InputActionAsset InputActions;
    public Celll Celll;

    public float STRCellDrain;

    private InputAction boostAction;

    private FloatTransfer transferBoost;
    private int ticker;

    private void OnEnable () {
        var map = InputActions.FindActionMap("Player");
        map.Enable ();
        boostAction = map.FindAction ( "Boost" );
        ticker = 0;
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "Player" ).Disable ();
    }

    void Update () {
        transferBoost.TryReset ( ticker );
        if ( boostAction.IsPressed() ) {
            if ( Celll.Drain ( STRCellDrain * Time.deltaTime ) ) {
                transferBoost.Add ( 1 );
            }
        }
        ticker++;
    }

    private void FixedUpdate () {
        float boostPower = transferBoost.GetValue ( ticker );

        if ( boostPower > 0 ) {
            FlightCoreBoost.enabled = true;
            FlightCoreNormal.enabled = false;
            FlightDriver.FlightCore = FlightCoreBoost;
        } else {
            FlightCoreNormal.enabled = true;
            FlightCoreBoost.enabled = false;
            FlightDriver.FlightCore = FlightCoreNormal;
        }
        FlightCoreBoost.Boost ();

        ticker = 0;
    }
}

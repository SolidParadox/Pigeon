using UnityEngine;
using UnityEngine.InputSystem;

public class ImpactWrench : MonoBehaviour {
    public InputActionAsset InputActions;
    public Rigidbody        rgb;

    public float            STR_push;

    private void OnEnable () {
        InputActions.FindActionMap("Wrench").Enable();
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "Wrench" ).Disable ();
    }

    void Update () {
        InputAction delta = InputActions.FindAction("PushObject");
        if ( delta.WasPressedThisDynamicUpdate() ) {
            rgb.AddForce ( rgb.transform.forward * STR_push , ForceMode.VelocityChange );
        }
    }
}

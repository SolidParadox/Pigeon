using UnityEngine;
using UnityEngine.InputSystem;

public class ImpactWrench : MonoBehaviour {
    public InputActionAsset InputActions;
    public Rigidbody        rgb;

    public float            STR_push;

    public bool toggleStatus = false;

    private void OnEnable () {
        toggleStatus = false;
        InputActions.FindActionMap("Wrench").Enable();
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "Wrench" ).Disable ();
    }

    void Update () {
        InputAction delta = InputActions.FindAction("PushObject");
        if ( delta.WasPressedThisDynamicUpdate () ) {
            toggleStatus = !toggleStatus;
        }
        if ( toggleStatus ) {
            rgb.AddForce ( rgb.transform.forward * STR_push * Time.deltaTime , ForceMode.VelocityChange );
        }
    }
}

using System;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TweaksMenu : MonoBehaviour {
    public CanvasGroup cg;
    
    public InputActionAsset InputActions;
    private InputAction activationAction;
    private InputAction alternativeExitAction;

    public RectTransform [] cells;

    public FlightCore   FlightCore;
    public ImpactWrench ImpactWrench;
    public FlightDriver FlightDriver;
    public CameraStable CameraStable;

    public bool menuStatus;

    protected void SetDisplay ( int index, float value ) {
        cells [ index ].GetChild ( 4 ).GetComponent<Slider> ().value = value;
        cells [ index ].GetChild ( 2 ).GetComponent<TMP_Text> ().text = value.ToString("F2");
    }
    protected void SetDisplay ( int index , bool value ) {
        cells [ index ].GetChild ( 4 ).GetChild (0).GetComponent<TMP_Text>().text = value ? "ON" : "OFF";
    }

    protected void SetAllDisplays () {
        SetDisplay ( 0 , ImpactWrench.STR_push );
        SetDisplay ( 1 , FlightCore.STRwingflap );
        SetDisplay ( 2 , FlightCore.alignmentPower.magnitude );
        SetDisplay ( 3 , FlightCore.alignmentPower.x / ( FlightCore.alignmentPower.y != 0 ? FlightCore.alignmentPower.y : 1 ) );
        SetDisplay ( 4 , FlightCore.STRredirect );
        SetDisplay ( 5 , 1 );
        SetDisplay ( 6 , CameraStable.enabled );        // This one is a bool btw
        //SetDisplay ( 7 ,  );
        SetDisplay ( 8 , FlightDriver.maxInput );
        SetDisplay ( 9 , FlightCore.controlsSensitivity.x / ( FlightCore.controlsSensitivity.y != 0 ? FlightCore.controlsSensitivity.y : 1 ) );
        SetDisplay ( 10 , FlightCore.STRexpoDrag );
        //SetDisplay ( 11 ,  );
    }

    public void CallbackFunction ( int index , float evt ) {
        switch ( index ) {
            case 0:
                ImpactWrench.STR_push = evt;
                break;
            case 1:
                FlightCore.STRwingflap = evt;
                break;
            case 2:
                FlightCore.alignmentPower = FlightCore.alignmentPower.normalized * evt;
                break;
            case 3:
                //FlightCore.alignmentPower = 
                break;
            case 4:
                FlightCore.STRredirect = evt;
                break;
            case 5:
                break;
            case 8:
                FlightDriver.maxInput = evt;
                break;
            case 9:
                FlightCore.controlsSensitivity = FlightCore.controlsSensitivity.normalized * evt;
                break;
            case 10:
                FlightCore.STRexpoDrag = evt;
                break;
            // case 7 and 11 left empty if no property
            default:
                return;
        }
        Debug.Log ( index + " AAAAAa" );
        SetDisplay ( index , evt );
    }

    public void CallbackFunction ( int index ) {
        bool delta = false;
        if ( index == 6 ) { CameraStable.enabled = !CameraStable.enabled; delta = CameraStable.enabled; }
        SetDisplay ( index , delta );
    }

    public void SetCGStatus ( bool newStatus ) {
        menuStatus = newStatus;
        cg.alpha = newStatus ? 1 : 0;
        cg.interactable = newStatus;
        cg.blocksRaycasts = newStatus;
        if ( newStatus ) {
            Time.timeScale = 0.01f;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Start () {
        var map = InputActions.FindActionMap ( "UI" );
        map.Enable ();
        activationAction = map.FindAction ( "ActivateTweaks" );
        alternativeExitAction = map.FindAction ( "Cancel" );

        int cc = cg.transform.GetChild ( 0 ).GetChild ( 1 ).childCount;
        cells = new RectTransform [ cc ];

        for ( int i = 0; i < cc; i++ ) {
            cells [ i ] = cg.transform.GetChild ( 0 ).GetChild ( 1 ).GetChild ( i ).GetComponent<RectTransform> ();
            int index = i;
            // fucking capture groups
            try {
                cells [ i ].GetChild ( 4 ).GetComponent<Slider> ().onValueChanged.AddListener ( evt => CallbackFunction ( index , evt ) );
            } catch ( Exception e ) {
                cells [ i ].GetChild ( 4 ).GetComponent<Button> ().onClick.AddListener ( () => CallbackFunction ( index ) );
            }
        }

        SetAllDisplays ();

        SetCGStatus ( false );
    }

    private void OnDisable () {
        InputActions.FindActionMap ( "UI" ).Disable ();
    }

    private void Update () {
        if ( activationAction.WasPressedThisDynamicUpdate () ) {
            SetCGStatus( !menuStatus );
        }
        if ( menuStatus && alternativeExitAction.WasPressedThisDynamicUpdate () ) {
            SetCGStatus( false );
        }
    }
}

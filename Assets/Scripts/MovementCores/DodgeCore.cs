using UnityEngine;
using System.Collections;

public class DodgeCore : MonoBehaviour {
    public FlightDriver FlightDriver;
    public Rigidbody rgb;

    public float dodgeSpeed;
    public float dodgeDuration;

    private Vector3 dodge;
    public bool isDodging;

    public void RXRelative ( Vector3 alpha ) {
        RX ( rgb.transform.TransformDirection( alpha ) );
    }

    public void RX ( Vector3 alpha ) {
        dodge = alpha;
    }

    private void FixedUpdate () {
        if ( !isDodging && dodge != Vector3.zero ) {
            StartCoroutine ( DodgeRoutine ( dodge ) );
        }
        dodge = Vector3.zero;
    }

    private IEnumerator DodgeRoutine ( Vector3 direction ) {
        isDodging = true;
        FlightDriver.SetBoostInterdiction ( true );
        float timer = dodgeDuration;
        Vector3 savedVelocity = rgb.linearVelocity;

        rgb.linearVelocity = direction * dodgeSpeed;
        while ( timer > 0 ) {
            timer -= Time.fixedDeltaTime;
            // If we hit something we wont return to the same speed ( and possibly bounce / land )
            yield return new WaitForFixedUpdate ();
        }
        
        rgb.linearVelocity = savedVelocity; 
        FlightDriver.SetBoostInterdiction ( false );
        isDodging = false;
    }
}

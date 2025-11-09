using UnityEngine;

public class DodgeDriver : MonoBehaviour {
    public float dodgeSpeed;
    public float dodgeDistance;

    private bool dodge;

    public void RX ( bool alpha ) {
        dodge = alpha;
    }

    private void FixedUpdate () {
        if ( dodge ) {
            
        }
        dodge = false;
    }
}

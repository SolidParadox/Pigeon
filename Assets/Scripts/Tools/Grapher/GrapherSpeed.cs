using UnityEngine;

public class GrapherSpeed : Grapher {
    public Rigidbody rgb;

    public override void Update () {
        data [ index ] = rgb.linearVelocity.magnitude;
        base.Update ();
    }
}

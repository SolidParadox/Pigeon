using UnityEngine;

public class CameraStable : MonoBehaviour {
    void LateUpdate () {
        transform.localRotation = Quaternion.Euler ( 0 , 0 , -transform.parent.localRotation.eulerAngles.z );
    }

    private void OnDisable () {
        transform.localRotation = Quaternion.identity;
    }
}

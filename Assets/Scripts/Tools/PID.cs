using UnityEngine;

public class PID : MonoBehaviour {
    [Header("PID Gains")]
    public float Kp = 1f;
    public float Ki = 0f;
    public float Kd = 0f;

    [Header("Internal State")]
    public float integral;
    public float lastError;
    public float lastValueVelocity;

    public float integralDecay;

    public float Compute ( float targetValue , float currentValue , float? valueVelocity = null ) {
        float error = targetValue - currentValue;
        float dt = Time.fixedDeltaTime;

        integral += error * dt;
        integral -= integralDecay * integral * dt;

        float derivative;

        if ( valueVelocity.HasValue ) {
            derivative = -valueVelocity.Value;
            lastValueVelocity = valueVelocity.Value;
        } else {
            derivative = ( error - lastError ) / dt;
            lastValueVelocity = derivative;
        }

        lastError = error;

        if ( enabled ) Debug.Log ( integral );

        return Kp * error + Ki * integral + Kd * derivative;
    }

    public void Reset () {
        integral = 0f;
        lastError = 0f;
        lastValueVelocity = 0f;
    }
}

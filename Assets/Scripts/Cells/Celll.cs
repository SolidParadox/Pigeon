using UnityEngine;

public class Celll : MonoBehaviour {
    public  float   resourceMax;
    public  float   resourceCurrent;

    public  float   flatRate;
    public  float   loadRate;

    public  float   loadTimeDown;
    public  float   loadTimeDead;
    private float   timeLoad;

    public  bool    recovering;

    private void Start () {
        if ( !recovering ) {
            resourceCurrent = resourceMax;
        }
    }

    public bool IsAvailable () {
        return !recovering && resourceCurrent > 0;
    }

    void Update () {
        resourceCurrent += flatRate * Time.deltaTime;
        if ( recovering || timeLoad <= 0 ) { resourceCurrent += loadRate * Time.deltaTime; }
        if ( timeLoad > 0 ) {
            timeLoad -= Time.deltaTime;
            if ( timeLoad <= 0 && recovering ) {
                recovering = false;
            }
        }
        if ( resourceCurrent > resourceMax ) {
            resourceCurrent = resourceMax;
        }
    }

    public void Load ( float amount ) {
        resourceCurrent += amount;
        if ( timeLoad > 0 ) {
            timeLoad = loadTimeDown;
            recovering = false;
        }
        if ( resourceCurrent > resourceMax ) {
            resourceCurrent = resourceMax;
        }
    }

    public bool Drain ( float amount ) {
        if ( amount <= 0 ) return true;
        if ( !recovering && resourceCurrent - amount >= 0 ) {
            resourceCurrent -= amount;
            timeLoad = loadTimeDown;
            if ( resourceCurrent <= 0 ) {
                resourceCurrent = 0;
                recovering = true;
                timeLoad = loadTimeDead;
            }
            return true;
        }
        if ( resourceCurrent != 0 && !recovering ) {
            resourceCurrent = 0; recovering = true; timeLoad = loadTimeDead;
            return true;
        }
        return false;
    }

    public float VariDrain ( float amount ) {
        amount = Mathf.Min ( amount , resourceCurrent );
        if ( recovering ) amount = 0;
        Drain ( amount );
        return amount;
    }

    public float GetLoad () {
        if ( recovering ) return 0;
        return resourceCurrent;
    }

    public float GetAvailableLoad () {
        if ( resourceMax == 0 ) return 0;
        return GetLoad () / resourceMax;
    }
}
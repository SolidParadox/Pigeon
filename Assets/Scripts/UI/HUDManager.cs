using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Celll    Celll;
    public Slider   Slider;

    public void LateUpdate () {
        Slider.value = Celll.GetAvailableLoad ();
    }
}

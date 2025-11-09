using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Celll    Celll;
    public Slider   Slider;

    public Color alpha, beta;

    public void LateUpdate () {
        Slider.value = Celll.resourceCurrent / Celll.resourceMax;
    }
}

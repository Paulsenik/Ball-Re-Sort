using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour {

    public Image img;
    private float sliderValue = 0f;

    public void setValue(float value) {
        sliderValue = (value > 1 ? 1 : (value < 0 ? 0 : value));
        img.fillAmount = sliderValue;
    }

    public float getValue() {
        return sliderValue;
    }
}

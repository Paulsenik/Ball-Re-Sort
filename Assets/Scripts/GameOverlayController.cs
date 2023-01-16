using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlayController : MonoBehaviour
{

    public SliderController slider;
    private OptionController option;

    public void Start() {
        option = GameObject.FindObjectOfType<OptionController>();
    }

    public void showOptions() {
        option.showOptions();
    }

}

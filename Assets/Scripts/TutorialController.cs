using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    public Image countdownImage;

    public void skipTutorial() {
        GameObject.FindObjectOfType<LevelManager>().skipTutorial();
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    private GameManager gm;

    public enum MENU_STATE { MAIN, LEVEL, AUDIO, ABOUT }
    public MENU_STATE state = MENU_STATE.MAIN;
    private int levelPageIndex = 0;
    private int maxLevels;

    public Animator mainOverlay;
    public Animator levelOverlay;
    public Animator audioOverlay;
    public Animator aboutOverlay;

    public Slider soundSlider;
    public Slider musicSlider;

    [Header("Buttons")]
    public List<TextMeshProUGUI> levelButtons = new List<TextMeshProUGUI>();
    // TODO

    public void Start() {
        gm = GameObject.FindObjectOfType<GameManager>();
        maxLevels = gm.levelsInOrder.Count;
    }

    public void Update() {
        for (int i = 0; i < levelButtons.Count; i++) {
            levelButtons[i].text = (levelPageIndex * 15 + i + 1).ToString();
            if (15 * levelPageIndex + i >= maxLevels) {
                levelButtons[i].gameObject.SetActive(false);
            } else {
                levelButtons[i].gameObject.SetActive(true);
            }
        }
    }

    public void updateSliders() {
        AudioManager.instance.setVolume("Sound", soundSlider.value);
        AudioManager.instance.setVolume("Music", musicSlider.value);
    }

    public void actionBack() {
        switch (state) {
            case MENU_STATE.MAIN:
                break;
            case MENU_STATE.LEVEL:
                if (levelPageIndex == 0) {
                    state = MENU_STATE.MAIN;
                    mainOverlay.SetTrigger("show");
                    levelOverlay.SetTrigger("hide");
                } else {
                    levelPageIndex--;
                }
                break;
            case MENU_STATE.AUDIO:
                state = MENU_STATE.MAIN;
                mainOverlay.SetTrigger("show");
                audioOverlay.SetTrigger("hide");
                break;
            case MENU_STATE.ABOUT:
                state = MENU_STATE.MAIN;
                mainOverlay.SetTrigger("show");
                aboutOverlay.SetTrigger("hide");
                break;
        }
    }

    public void actionForward() {
        if (state == MENU_STATE.LEVEL) {
            if ((levelPageIndex + 1) * 15 < maxLevels) {
                levelPageIndex++;
                Debug.Log("levelPage ++");
            } else {
                Debug.Log("max levelPage reached!");
            }
        } else
            Debug.Log("action Forward. NO ACTION FOUND");
    }

    public void actionLevelLoad(int buttonIndex) {
        Debug.Log((buttonIndex + 15 * levelPageIndex));
        gm.loadLevel(buttonIndex + 15 * levelPageIndex);
    }

    public void actionPlay() {
        if (state == MENU_STATE.MAIN) {
            state = MENU_STATE.LEVEL;
            Debug.Log("play");
            mainOverlay.SetTrigger("hide");
            levelOverlay.SetTrigger("show");
        }
    }

    public void actionAbout() {
        if (state == MENU_STATE.MAIN) {
            state = MENU_STATE.ABOUT;
            mainOverlay.SetTrigger("hide");
            aboutOverlay.SetTrigger("show");
        }
    }

    public void actionAudioSettings() {
        if (state == MENU_STATE.MAIN) {
            state = MENU_STATE.AUDIO;
            mainOverlay.SetTrigger("hide");
            audioOverlay.SetTrigger("show");
        }
    }

}

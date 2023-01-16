using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class OptionController : MonoBehaviour {

    [Header("Attributes")]
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI levelTime;

    [HideInInspector]
    public Animator anim;
    private GameManager gm;
    private LevelManager lm;


    public void Start() {
        anim = GetComponent<Animator>();
        gm = GameObject.FindObjectOfType<GameManager>();
        lm = GameObject.FindObjectOfType<LevelManager>();
    }

    private void Update() {
        if (gm.isInMenu()) {
            levelTime.text = "";
        } else if (lm != null && !lm.completed) {
            double d = Math.Round(lm.getTotalTime(), 1);
            levelName.text = gm.getCurrentLevel();
            levelTime.text = "Time: " + d + (d % 1 == 0 ? ",0" : "") + "s";
        }
    }

    public void actionRetry() {
        gm.restartLevel();
    }

    public void actionNext() {
        gm.loadNextLevel();
    }

    public void actionMenu() {
        gm.loadMenu();
    }

    public void toggleOptions() {
        if (anim.GetBool("Shown")) {
            hideOptions();
        } else {
            showOptions();
        }
    }

    public void showOptions() {
        if (!anim.GetBool("Shown")) {
            gm = GameObject.FindObjectOfType<GameManager>();
            levelName.text = gm.getCurrentLevel();

            anim.SetBool("Shown", true);
            if (!gm.isInMenu())
                lm.pauseGame(anim.GetBool("Shown"));
        }
    }

    public void hideOptions() {
        if (anim.GetBool("Shown")) {
            anim.SetBool("Shown", false);
            if (!gm.isInMenu())
                lm.pauseGame(anim.GetBool("Shown"));
        }
    }
}

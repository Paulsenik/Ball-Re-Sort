using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    [Header("MENU")]
    public bool isMenu = false;
    public float multiplierMAX = 10f;
    public float multiplierCooldown = 5f;
    private float multiplierCount = 0f;
    private Unity.Mathematics.Random rand;

    [Header("User Dragspeed")]
    public float dragSpeed = 75.0f;

    [Header("Pulling")]
    public float pullSpeed = 1.0f;
    public bool paused = false;

    [Header("ColorMultiplier")]
    public float pullMultiplierRED = 1.0f;
    public float pullMultiplierGREEN = 1.0f;
    public float pullMultiplierBLUE = 1.0f;

    private float baseMultiplier = 10f;

    [Header("Pulling Cutoff")]
    public float pullDistanceCutoff = 0.02f;
    public float dragDistanceCutoff = 0.02f;

    [Header("Collding")]
    public float collisionCountdown = 5f;
    private float lastCollsion = 0f;
    private float totalTime = 0f;
    private SliderController slider;
    [HideInInspector]
    public bool completed = false;

    [Header("Animation")]
    private Animator gameOverlayAnim;
    private OptionController option;

    [Header("Particle")]
    public float spawnCooldown = 0.5f;
    private float spawnCounter = 0f;

    [Header("Tutorial")]
    public float duration;
    private float countdown = 0;
    private Animator tutorialAnim;
    private Image tutorialTimeVisual;

    public void Start() {
        option = GameObject.FindObjectOfType<OptionController>();
        GameOverlayController goc = GameObject.FindObjectOfType<GameOverlayController>();
        if (goc != null) {
            gameOverlayAnim = goc.GetComponent<Animator>();
            slider = goc.slider;
        }

        rand = new Unity.Mathematics.Random();
        rand.InitState();

        // Tutorial
        GameObject tutorialOverlay = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorialOverlay != null) {
            tutorialTimeVisual = tutorialOverlay.GetComponent<TutorialController>().countdownImage;
            tutorialAnim = tutorialOverlay.GetComponent<Animator>();
            tutorialAnim.SetBool("hidden", false);
        }
    }

    public void Update() {

        if (isMenu) {
            menuAction();
            return;
        }

        if (tutorialAnim != null) {
            if (countdown > duration) { // tutorial hidden
                if (!tutorialAnim.GetBool("hidden")) {
                    tutorialAnim.SetBool("hidden", true);
                    pauseGame(false);
                }
            } else { // tutorial shown
                if (!option.anim.GetBool("Shown")) {
                    countdown += Time.deltaTime;
                    float f = countdown / duration;
                    tutorialTimeVisual.fillAmount = (f < 0 ? 0 : (f > 1 ? 1 : f));
                }
                pauseGame(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape)) {
            option.toggleOptions();
        }

        if (!completed && !paused) { // game is running

            totalTime += Time.deltaTime;

            if (doSameColorsCollide()) {
                lastCollsion = 0f;
            } else {
                lastCollsion += Time.deltaTime;
            }

            float completionValue = lastCollsion / collisionCountdown;

            slider.setValue(completionValue);

            if (completionValue >= 1) {
                completed = true;
                triggerCompletedAnimation();
            }
        }

        if (completed) {
            spawnCounter += Time.deltaTime;
            if (spawnCounter > spawnCooldown) {
                spawnCounter = 0;
                foreach (PullObject po in PullObject.objs) {
                    po.spawnParticle();
                }
            }
        }
    }

    void FixedUpdate() {
        foreach (PullObject e1 in PullObject.objs) {

            Rigidbody2D r2d = e1.gameObject.GetComponent<Rigidbody2D>();

            if (!e1.isDragged() || !e1.interactable) {
                if (e1.pullable) {

                    Vector2 dir = new Vector2();

                    foreach (PullObject e2 in PullObject.objs) {

                        PullObject.ColorGroup col = e2.getSameColorGroup(e1);
                        if (e1 != e2 && col != PullObject.ColorGroup.NULL) { // do not attract to itself and only to same color
                            dir += (Vector2)(e2.gameObject.transform.position - e1.gameObject.transform.position).normalized;
                        }
                    }

                    // prevent jiggling
                    if (dir.magnitude <= pullDistanceCutoff) {
                        dir = new Vector2(0, 0);
                    }

                    r2d.velocity = dir.normalized * pullSpeed * Time.fixedDeltaTime * baseMultiplier * getStrongestPullMultiplier(e1);
                }
            } else if (e1.interactable) {
                Vector2 vN = (e1.getTargetPos() - ((Vector2)e1.gameObject.transform.position)) * dragSpeed * Time.fixedDeltaTime * baseMultiplier;

                if (vN.magnitude < dragDistanceCutoff) {
                    vN = new Vector2();
                }

                r2d.velocity = vN;
            }
        }
    }

    public void menuAction() {
        multiplierCount += Time.deltaTime;

        //Debug.Log("tst1 " + multiplierCount);
        if (multiplierCount > multiplierCooldown) {
            multiplierCount = 0f;
            foreach (PullObject obj in PullObject.objs) {
                obj.rb2D.AddForce(new Vector2(rand.NextFloat(-multiplierMAX, multiplierMAX), rand.NextFloat(-multiplierMAX, multiplierMAX)));
            }
        }
    }

    public void skipTutorial() {
        countdown = duration + 1;
    }

    public void triggerCompletedAnimation() {
        gameOverlayAnim.SetTrigger("Completed");
        AudioManager.instance.play("String_1");
    }

    public bool doSameColorsCollide() {
        foreach (PullObject obj in PullObject.objs)
            if (obj.isColliding())
                return true;
        return false;
    }

    private float getPullMultiplier(PullObject.ColorGroup color) {
        switch (color) {
            case PullObject.ColorGroup.NULL:
                return 0;
            case PullObject.ColorGroup.RED:
                return pullMultiplierRED;
            case PullObject.ColorGroup.GREEN:
                return pullMultiplierGREEN;
            case PullObject.ColorGroup.BLUE:
                return pullMultiplierBLUE;
            default:
                Debug.LogError("Colormultiplier not defined");
                break;
        }
        return 0;
    }

    private float getStrongestPullMultiplier(PullObject p) {
        float strongest = int.MinValue;
        foreach (PullObject.ColorGroup cg in p.colors) {
            float mul = getPullMultiplier(cg);
            if (mul > strongest)
                strongest = mul;
        }
        return strongest;
    }

    //Only pauses Physics
    public void pauseGame(bool b) {
        if (paused == b)
            return;

        paused = b;
        if (!b || (b && !completed)) {
            foreach (PullObject obj in PullObject.objs) {
                obj.GetComponent<Rigidbody2D>().simulated = !b;
            }
        }
    }

    public float getTotalTime() {
        return totalTime;
    }
}

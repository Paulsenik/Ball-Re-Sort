using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("SceneManager")]
    public string menuScene;
    public List<string> levelsInOrder = new List<string>();
    private Animator sceneTrans;
    private bool isMenu = false;
    private int currentLevel = 0;

    // animation
    private string sceneToLoad = "";

    public void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        sceneTrans = GetComponent<Animator>();
    }

    public void Update() {
        isMenu = SceneManager.GetActiveScene().name.Equals(menuScene);
    }

    public void restartLevel() {
        if (!isMenu)
            loadLevel(currentLevel);
    }

    public void loadNextLevel() {
        if (levelsInOrder.Count == 0)
            return;

        if (isMenu) {
            currentLevel = 0;
            loadLevel(currentLevel);
        } else {
            if (currentLevel + 1 >= levelsInOrder.Count) {
                Debug.Log("next: Menu " + currentLevel + " " + levelsInOrder.Count);
                loadMenu();
            } else {
                loadLevel(currentLevel + 1);
                Debug.Log("next: " + currentLevel);
            }
        }
    }

    public void loadMenu() {
        if (!isMenu)
            try {
                PullObject.objs.Clear();
                if (sceneTrans == null) {
                    sceneToLoad = "";
                    SceneManager.LoadScene(menuScene);
                } else {
                    sceneToLoad = menuScene;
                    sceneTrans.SetTrigger("Transition");
                }
            } catch (Exception e) {
                Debug.LogError(e.StackTrace);
            }
    }

    public void loadLevel(int index) {
        // no levels
        if (levelsInOrder.Count == 0)
            return;

        // wrong input
        if (index >= levelsInOrder.Count || index < 0)
            return;

        try {
            PullObject.objs.Clear();
            currentLevel = index;
            if (sceneTrans == null) {
                sceneToLoad = "";
                SceneManager.LoadScene(levelsInOrder[index]);
            } else {
                sceneToLoad = levelsInOrder[index];
                sceneTrans.SetTrigger("Transition");
            }
        } catch (Exception e) {
            Debug.LogError(e.StackTrace);
            return;
        }
    }

    // ONLY ANIMATION
    public void loadSceneToLoad() {
        if (sceneToLoad != null && !sceneToLoad.Equals(""))
            SceneManager.LoadScene(sceneToLoad);
    }

    public String getCurrentLevel() {
        if (isMenu) {
            return "Menu";
        } else {
            return "Level " + (currentLevel + 1);
        }
    }

    public bool isInMenu() {
        return isMenu;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorDisplay : MonoBehaviour {
    private PullObject p;

    private void Start() {
        p = GetComponent<PullObject>();
    }

    void Update() {
        for (int i = 0; i < p.colors.Count; i++) {
            p.setColor(i, getColor(p.colors[i]));
            p.trail[i].startColor = getColor(p.colors[i]);
            p.trail[i].endColor = p.trail[i].startColor;
        }
        p.interactableTexture.enabled = !p.interactable;
        p.pullableTexture.enabled = !p.pullable;
    }

    public static Color getColor(PullObject.ColorGroup cGroup) {
        switch (cGroup) {
            case PullObject.ColorGroup.NULL:
                return Color.white;
            case PullObject.ColorGroup.RED:
                return Color.red;
            case PullObject.ColorGroup.GREEN:
                return Color.green;
            case PullObject.ColorGroup.BLUE:
                return Color.blue;
            default:
                return Color.black;
        }
    }
}

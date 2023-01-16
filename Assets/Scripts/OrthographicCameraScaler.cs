using UnityEngine;

[ExecuteInEditMode]
public class OrthographicCameraScaler : MonoBehaviour {
    // https://gamedev.stackexchange.com/questions/167317/scale-camera-to-fit-screen-size-unity

    // Set this to the in-world distance between the left & right edges of your scene

    public enum TF { WIDTH, HEIGHT, WIDTH_AND_HEIGHT };

    public TF ScaleByFixed = TF.WIDTH;

    public float totalScale;

    public float scaleHorizontal = 10;
    public float scaleVertical = 10;

    Camera cam;
    void Start() {
        cam = GetComponent<Camera>();
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.   
    void Update() {

        if (TF.WIDTH == ScaleByFixed) {
            adjustWithFixedWidth();
        } else if (TF.HEIGHT == ScaleByFixed) {
            adjustWithFixedHeight();
        } else if (TF.WIDTH_AND_HEIGHT == ScaleByFixed) {
            if ((scaleHorizontal / scaleVertical) > ((float)Screen.width / Screen.height)) {
                adjustWithFixedWidth();
            } else {
                adjustWithFixedHeight();
            }
        }
    }

    private void adjustWithFixedWidth() {
        float unitsPerPixel = (scaleHorizontal * totalScale) / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;
    }

    private void adjustWithFixedHeight() {
        float unitsPerPixel = (scaleVertical * totalScale) / Screen.height;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredHalfHeight;
    }
}

/*

[CustomEditor(typeof(OrthographicCameraScaler))]
public class MyScriptEditor : Editor {

    public void OnInspectorGUI() {
        var myScript = target as OrthographicCameraScaler;

        myScript.ScaleByFixed = (OrthographicCameraScaler.TF)EditorGUILayout.EnumPopup("Scale camera by fixed", myScript.ScaleByFixed);
        myScript.totalScale = EditorGUILayout.Slider("Scale:", myScript.totalScale, 0.1f, 10);

        if (myScript.ScaleByFixed == OrthographicCameraScaler.TF.WIDTH_AND_HEIGHT) {
            myScript.scaleHorizontal = EditorGUILayout.Slider("WIDTH:", myScript.scaleHorizontal, 1, 100);
            myScript.scaleVertical = EditorGUILayout.Slider("HEIGHT:", myScript.scaleVertical, 1, 100);
        } else if (myScript.ScaleByFixed == OrthographicCameraScaler.TF.WIDTH) {
            myScript.scaleHorizontal = EditorGUILayout.Slider("WIDTH:", myScript.scaleHorizontal, 1, 100);
        } else if (myScript.ScaleByFixed == OrthographicCameraScaler.TF.HEIGHT) {
            myScript.scaleVertical = EditorGUILayout.Slider("HEIGHT:", myScript.scaleVertical, 1, 100);
        }

    }
}
*/
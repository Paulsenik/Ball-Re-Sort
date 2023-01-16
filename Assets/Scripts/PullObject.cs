using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PullObject : MonoBehaviour {

    public static List<PullObject> objs = new List<PullObject>();

    // can be pulled and pushed by other PullObjects
    public bool pullable = true;
    // can be used by player
    public bool interactable = true;

    public enum ColorGroup { NULL, RED, GREEN, BLUE }

    // Public
    public List<ColorGroup> colors = new List<ColorGroup>();

    [Header("Sprites")]
    public List<SpriteRenderer> colorTextures = new List<SpriteRenderer>();
    public SpriteRenderer interactableTexture;
    public SpriteRenderer pullableTexture;

    [Header("Effect")]
    public GameObject particle;
    public GameObject collisionParticle;
    public List<TrailRenderer> trail = new List<TrailRenderer>();
    public Behaviour halo;

    // Private
    [HideInInspector]
    public Rigidbody2D rb2D;
    private bool mouseDrag = false;
    private Vector2 targetPos;
    private List<PullObject> collidedObjects = new List<PullObject>();

    void Start() {
        objs.Add(this);
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private int index = 0;

    public void spawnParticle() {
        particle.transform.position = transform.position;
        if (colors.Count != 0) {
            particle.GetComponent<ParticleSystem>().startColor = ColorDisplay.getColor(colors[index++]);
            index = (index >= colors.Count ? 0 : index);
        }
        Instantiate(particle);
    }

    public void spawnCollisionParticle(Vector2 pos) {
        collisionParticle.transform.position = pos;
        if (colors.Count != 0) {
            collisionParticle.GetComponent<ParticleSystem>().startColor = ColorDisplay.getColor(colors[index++]);
            index = (index >= colors.Count ? 0 : index);
        }
        Instantiate(collisionParticle);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        PullObject obj = collision.gameObject.GetComponent<PullObject>();

        if (obj != null) {

            // CollsionParticle
            Vector2 colPos = collision.transform.position + 0.5f * (transform.position - collision.gameObject.transform.position);
            spawnCollisionParticle(colPos);

            ColorGroup colorSame = obj.getSameColorGroup(this);
            if (colorSame != ColorGroup.NULL) {

                collidedObjects.Add(obj);
                if (!halo.enabled) {
                    halo.enabled = true;
                }
            }
        }
        AudioManager.instance.playRandom("Step_1", "Step_2", "Step_2");
    }

    private void OnCollisionExit2D(Collision2D collision) {
        PullObject obj = collision.gameObject.GetComponent<PullObject>();
        if (obj != null) {
            if (obj.getSameColorGroup(this) != ColorGroup.NULL) {
                collidedObjects.Remove(obj);
                if (collidedObjects.Count == 0)
                    halo.enabled = false;
            }
        }
    }

    public void setColor(int spriteIndex, Color color) {
        if (spriteIndex >= 0 && spriteIndex < colorTextures.Count)
            colorTextures[spriteIndex].color = color;
    }

    public bool isColliding() {
        return collidedObjects.Count != 0;
    }

    private void OnMouseDrag() {
        mouseDrag = true;
        targetPos = Camera.main.ScreenToWorldPoint(returnValidMousePos(Input.mousePosition));
    }

    private Vector2 returnValidMousePos(Vector2 vector) {
        float x = vector.x;
        float y = vector.y;

        x = (x < 0 ? 0 : (x > Screen.width ? Screen.width : x));
        y = (y < 0 ? 0 : (y > Screen.height ? Screen.height : y));

        return new Vector2(x, y);
    }

    private void OnMouseUp() {
        mouseDrag = false;
        if (!pullable) {
            rb2D.velocity = new Vector2();
        }
    }

    public bool isDragged() {
        return mouseDrag;
    }

    public Vector2 getTargetPos() {
        return targetPos;
    }

    public ColorGroup getSameColorGroup(PullObject ob2) {
        foreach (ColorGroup g1 in colors)
            if (ob2.colors.Contains(g1))
                return g1;
        return ColorGroup.NULL;
    }
}

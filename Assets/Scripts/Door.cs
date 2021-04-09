using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update

    [HideInInspector]
    public  Room room;

    public delegate void PassingDoorAction(Door door);
    public static event PassingDoorAction OnPassingThrough;
    private bool locked = true;

    private float openAngle = -20f;
    private float wideAngle = -90f;
    private float startRotation;
    private IEnumerator flipCoroutine;

    [SerializeField]
    private GameObject lightObject;
    [SerializeField]
    private Transform doorPivot;

    [SerializeField]
    private AnimationCurve openCurve;
    [SerializeField]
    private AnimationCurve closeCurve;
    public bool Locked {
        get { return locked; }
        set {
            locked = value;
            if (locked) {
                Close();
            } else {
                Open();
            }
        }
    }

    public bool Focused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void Close() {
        lightObject.SetActive(false);
        StartCoroutine(Flipping(startRotation, .5f, closeCurve));
    }

    void Open() {
        lightObject.SetActive(true);
        StartCoroutine(Flipping(startRotation + openAngle, .5f, openCurve));
    }
    void Start()
    {
        lightObject.SetActive(false);
        startRotation = doorPivot.eulerAngles.y;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            Locked = !locked;
        }
    }

    void Opening() {
        //TODO: Go to next room with player
        OnPassingThrough(this);
        StartCoroutine(OpenAnimation());
        //YRotation = startRotation + wideAngle;
    }

    public void Interact()
    {
        if (locked) return;
        Opening();
    }

    public float YRotation {
        get => doorPivot.eulerAngles.y;
        set => doorPivot.rotation = Quaternion.Euler(doorPivot.eulerAngles.x, value, doorPivot.eulerAngles.z);

    }

    public IEnumerator Flipping(float endRotation, float duration, AnimationCurve curve) {


        float index = 0;
        float begin = YRotation;
        if (begin > 180) begin -= 360;
        while (index < duration) {
            index += Time.deltaTime;
            YRotation = Mathf.LerpUnclamped(begin, endRotation, curve.Evaluate(index / duration));
            yield return new WaitForFixedUpdate();
        }
        YRotation = endRotation;
    }

    public IEnumerator OpenAnimation() {
        yield return StartCoroutine(Flipping(startRotation + wideAngle, .8f, openCurve));
        yield return StartCoroutine(Flipping(startRotation, .3f, closeCurve));
    }
}

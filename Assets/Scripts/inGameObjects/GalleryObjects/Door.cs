using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractabelObject
{
    // Start is called before the first frame update

    [HideInInspector]
    public Room room;



    public delegate void PassingDoorAction(Door door);
    public static event PassingDoorAction OnPassingThrough;
    private bool locked = true;

    private float openAngle = 30f;
    private float wideAngle = 90f;
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
            if (locked == value) return;
            locked = value;
            if (locked) {
                Close();
            } else {
                Open();
            }
        }
    }

    void Close() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_closing);
        lightObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(Flipping(startRotation, .5f, closeCurve));
    }

    void Open() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_squeek, .2f);
        lightObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Flipping(startRotation + openAngle, 2f, openCurve));
    }
    void Start()
    {
        lightObject.SetActive(false);
        startRotation = doorPivot.eulerAngles.y;
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.O)) {
        //     Locked = !locked;
        // }
        // if (Input.GetKeyDown(KeyCode.P)) {
        //     StartCoroutine(OpenAnimation());
        // }
    }

    void Opening() {
        //TODO: Go to next room with player
        OnPassingThrough?.Invoke(this);
        StopAllCoroutines();
        StartCoroutine(OpenAnimation());
        //YRotation = startRotation + wideAngle;
    }

    public override void Interact()
    {
        if (locked) return;
        Opening();
    }

    protected override void OnFocus()
    {
        if (locked) return;
        base.OnFocus();
    }

    public float YRotation {
        get => doorPivot.eulerAngles.y;
        set => doorPivot.rotation = Quaternion.Euler(doorPivot.eulerAngles.x, value, doorPivot.eulerAngles.z);

    }

    public IEnumerator Flipping(float endRotation, float duration, AnimationCurve curve) {


        float index = 0;
        float begin = YRotation;
        //if (begin > 180) begin -= 360;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            YRotation = Mathf.LerpUnclamped(begin, endRotation, curve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        YRotation = endRotation;
    }

    public IEnumerator OpenAnimation() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_open);
        yield return StartCoroutine(Flipping(startRotation + wideAngle, .8f, openCurve));
        AudioHandler.Instance?.PlaySound(SFXFiles.door_closing);
        Close();
    }
}

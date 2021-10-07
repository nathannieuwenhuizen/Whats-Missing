using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractabelObject
{
    // Start is called before the first frame update

    [HideInInspector]
    public Room room;



    public delegate void DoorAction(Door door);
    public static event DoorAction OnPassingThrough;
    public static event DoorAction OnDoorOpen;
    private bool locked = true;

    private float openAngle = 30f;
    private float wideAngle = 90f;
    private float startAngle;

    private IEnumerator flipCoroutine;

    [SerializeField]
    private Transform doorPivot;
    [SerializeField]
    private AnimationCurve openCurve;
    [SerializeField]
    private AnimationCurve closeCurve;

    private bool flipped = false;

    [SerializeField]
    private float walkDistance = 4f;

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

    private void Close() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_closing);
        StopAllCoroutines();
        StartCoroutine(Flipping(startAngle, .5f, closeCurve));
    }

    private void Open() {
        OnDoorOpen?.Invoke(this);
        AudioHandler.Instance?.PlaySound(SFXFiles.door_squeek, .2f);
        StopAllCoroutines();
        StartCoroutine(Flipping(startAngle + openAngle, 2f, openCurve));
    }
    void Start()
    {
        startAngle = YRotation;
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.O)) {
        //     Locked = !locked;
        // }
        // if (Input.GetKeyDown(KeyCode.P)) {
        //     StartCoroutine(OpenAnimation());
        // }
    }
    private void CheckAngle() {
        if (room.Player != null) {
            float angle =  Vector3.Dot(transform.forward, room.Player.transform.position - transform.position);
            flipped = angle > 0;
        }
    }

    private void Opening() {
        //TODO: Go to next room with player
        CheckAngle();
        OnPassingThrough?.Invoke(this);
        CheckAngle();
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
        get => doorPivot.localRotation.eulerAngles.y;
        set => doorPivot.localRotation = Quaternion.Euler(doorPivot.localRotation.eulerAngles.x, value, doorPivot.localRotation.eulerAngles.z);

    }

    public IEnumerator Flipping(float endRotation, float duration, AnimationCurve curve) {

        CheckAngle();

        float index = 0;
        // float begin = YRotation;
        //if (begin > 180) begin -= 360;
        Quaternion start = doorPivot.localRotation;
        Quaternion end = Quaternion.Euler(doorPivot.localRotation.x, endRotation  * (flipped ? - 1 : 1), doorPivot.localRotation.z);
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            doorPivot.localRotation = Quaternion.Slerp(start, end, curve.Evaluate(index / duration));
            // YRotation = Mathf.LerpUnclamped(begin, endRotation * (flipped ? - 1 : 1), curve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        doorPivot.localRotation = end;

        // YRotation = endRotation;
    }

    public IEnumerator OpenAnimation() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_open);
        yield return StartCoroutine(Flipping(startAngle + wideAngle, 1.3f, openCurve));
        AudioHandler.Instance?.PlaySound(SFXFiles.door_closing);
        Close();
    }

    public Vector3 StartPos() {
        return transform.position + transform.forward * walkDistance - transform.right * 1f;
    }
    public Vector3 EndPos() {
        return transform.position - transform.forward * walkDistance - transform.right * 1f;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(StartPos(), .5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(EndPos(), .5f);

        Debug.DrawLine(StartPos(), EndPos(), Color.white);
    }
}

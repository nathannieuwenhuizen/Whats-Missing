using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractabelObject
{
    // Start is called before the first frame update

    [HideInInspector]
    public Room room;

    private bool inAnimation = false;

    private bool animated = false;


    public delegate void DoorAction(Door door);
    public static event DoorAction OnPassingThrough;
    public static event DoorAction OnDoorOpen;
    private bool locked = true;

    private float openAngle = 30f;
    private float wideAngle = 90f;
    private float startAngle;

    private IEnumerator flipCoroutine;
    private static AnimationCurve walkingCurve = AnimationCurve.EaseInOut(0,0,1,1);

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
        if (Animated) {
            AudioHandler.Instance?.Player3DSound(SFXFiles.door_closing, transform);
            StopAllCoroutines();
            StartCoroutine(AnimateDoorAngle(startAngle, .5f, closeCurve));
        } else {
            doorPivot.localRotation = Quaternion.Euler(doorPivot.localRotation.x, 0, doorPivot.localRotation.z);
        }
    }

    private void Open() {
        OnDoorOpen?.Invoke(this);
        
        AudioHandler.Instance?.PlaySound(SFXFiles.door_squeek, .2f);
        StopAllCoroutines();
        if (gameObject.activeSelf) StartCoroutine(AnimateDoorAngle(startAngle + openAngle, 2f, openCurve));
    }
    void Awake()
    {
        startAngle = doorPivot.localRotation.eulerAngles.y;
    }

    private bool CheckAngle() {
        if (room.Player != null) {
            float angle =  Vector3.Dot(transform.forward, room.Player.transform.position - transform.position);
            return angle > 0;
        }
        return false;
    }

    private void GoingThrough() {
        inAnimation = true;
        flipped = CheckAngle();
        OnPassingThrough?.Invoke(this);
        flipped = CheckAngle();
        StopAllCoroutines();
        StartCoroutine(GoingThroughAnimation());
    }

    public override void Interact()
    {
        if (locked || inAnimation) return;
        GoingThrough();
    }

    protected override void OnFocus()
    {
        if (locked || inAnimation) return;
        base.OnFocus();
    }

    public IEnumerator AnimateDoorAngle(float endRotation, float duration, AnimationCurve curve) {
        
        //to make sure the door flips away from the player.
        flipped = CheckAngle();

        float index = 0;
        Quaternion start = doorPivot.localRotation;
        Quaternion end = Quaternion.Euler(doorPivot.localRotation.x, endRotation  * (flipped ? - 1 : 1), doorPivot.localRotation.z);
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            doorPivot.localRotation = Quaternion.Slerp(start, end, curve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        doorPivot.localRotation = end;
    }

    ///<summary>
    /// Animates the player walking through the door
    ///</summary>
    public static IEnumerator Walking(Vector3 endPos, float duration, Player player) {
        float index = 0;
        player.Movement.EnableWalk = false;
        player.Movement.CharacterAnimator.SetTrigger("openingDoor");
        Vector3 begin = player.transform.position;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            SetPlayerPos(Vector3.LerpUnclamped(begin, endPos, walkingCurve.Evaluate(index / duration)), player);
            yield return new WaitForEndOfFrame();
        }
        player.Movement.EnableWalk = true;
        SetPlayerPos(endPos, player);
    }

    private static void SetPlayerPos(Vector3 value, Player player) {
        player.transform.position = new Vector3(value.x, player.transform.position.y, value.z);
    }
 

    public IEnumerator GoingThroughAnimation() {
        AudioHandler.Instance?.PlaySound(SFXFiles.door_open);
        yield return StartCoroutine(AnimateDoorAngle(startAngle + wideAngle, 1.3f, openCurve));
        yield return StartCoroutine(AnimateDoorAngle(startAngle + openAngle, .5f, openCurve));
        inAnimation = false;
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

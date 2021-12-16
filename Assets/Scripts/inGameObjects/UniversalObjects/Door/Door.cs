using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractabelObject
{

    
    [SerializeField]
    public Vector3 point0, point1, point2 = new Vector3();
    

    [HideInInspector]
    public Room room;

    private bool inAnimation = false;

    private bool animated = false;
    public static bool IN_WALKING_ANIMATION = false;


    public delegate void DoorAction(Door door);
    public static event DoorAction OnPassingThrough;
    public static event DoorAction OnDoorOpen;
    [HideInInspector]
    public bool locked = true;

    private float openAngle = 30f;
    private float wideAngle = 90f;
    private float startAngle;

    private IEnumerator flipCoroutine;
    private static AnimationCurve walkingCurve = AnimationCurve.EaseInOut(0,0,1,1);

    [SerializeField]
    private Transform doorPivot;
    public Transform DoorPivot {
        get { return doorPivot;}
    }
    [SerializeField]
    private AnimationCurve openCurve;
    [SerializeField]
    private AnimationCurve closeCurve;

    private bool flipped = false;

    [SerializeField]
    private float walkDistance = 4f;

    public virtual bool Locked {
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
            AudioHandler.Instance?.Play3DSound(SFXFiles.door_closing, transform);
            StopAllCoroutines();
            StartCoroutine(AnimateDoorAngle(startAngle, .5f, closeCurve));
        } else {
            SetDoorLocalRotation(Quaternion.Euler(doorPivot.localRotation.x, 0, doorPivot.localRotation.z));
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

    public void GoingThrough() {
        flipped = CheckAngle();
        StopAllCoroutines();
        StartCoroutine(GoingThroughFlippingAnimation());
    }

    public override void Interact()
    {
        if (locked || inAnimation || IN_WALKING_ANIMATION) return;
        flipped = CheckAngle();
        OnPassingThrough?.Invoke(this);
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
            SetDoorLocalRotation(Quaternion.Slerp(start, end, curve.Evaluate(index / duration)));
            // doorPivot.localRotation = Quaternion.Slerp(start, end, curve.Evaluate(index / duration));
            yield return new WaitForEndOfFrame();
        }
        SetDoorLocalRotation(end);
    }

    public virtual void SetDoorLocalRotation(Quaternion val) {
        doorPivot.localRotation = val;
    }

    ///<summary>
    /// Animates the player walking through the door
    ///</summary>
    public IEnumerator Walking(float duration, Player player) {
        IN_WALKING_ANIMATION = true;
        float index = 0;
        player.Movement.EnableWalk = false;
        player.Movement.RB.isKinematic = true;
        player.CharacterAnimationPlayer.SetTrigger("openingDoor");
        Vector3 endPos = GetFarthestPoint(player);
        SetBezierPoints(player);

        Vector3 begin = player.transform.position;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            UpdatePlayerWalkingPosition(walkingCurve.Evaluate(index / duration), player);
            yield return new WaitForEndOfFrame();
        }
        player.Movement.EnableWalk = true;
        player.Movement.RB.isKinematic = false;

        // SetPlayerPos(endPos, player);

        while(inAnimation) yield return new WaitForEndOfFrame();
        
        IN_WALKING_ANIMATION = false;
    }




    public virtual void UpdatePlayerWalkingPosition(float precentage, Player player) {
        Vector3 newPos =  Extensions.CalculateQuadraticBezierPoint(walkingCurve.Evaluate(precentage), point0, point1, point2);
        SetPlayerPos(newPos, player);
    }

    public Vector3 GetClosestPoint(Player player) {
        if (Vector3.Distance(EndPos(), player.transform.position) <Vector3.Distance(StartPos(), player.transform.position)) {
            return EndPos();
        } 
        return StartPos();
    }
    public Vector3 GetFarthestPoint(Player player) {
        if (Vector3.Distance(EndPos(), player.transform.position) > Vector3.Distance(StartPos(), player.transform.position)) {
            return EndPos();
        } 
        return StartPos();
    }

    private static void SetPlayerPos(Vector3 value, Player player) {
        player.transform.position = new Vector3(value.x, player.transform.position.y, value.z);
    }
 

    public IEnumerator GoingThroughFlippingAnimation() {
        inAnimation = true;
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

    public virtual void SetBezierPoints(Player player) {
        point0 = player.transform.position;
        point1 = GetClosestPoint(player);
        point2 = GetFarthestPoint(player);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(StartPos(), .2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(EndPos(), .2f);

        // Debug.DrawLine(StartPos(), EndPos(), Color.white);

        // if (inWalkingAnimation) {
            int numberOfPoints = 50;
            Gizmos.DrawWireSphere(point0, .5f);
            Gizmos.DrawWireSphere(point1, .5f);
            Gizmos.DrawWireSphere(point2, .5f);
            Vector3 beginPos = point0;
            if (point0 != null && point1 != null && point2 != null) {

                for (int i = 1; i < numberOfPoints + 1; i++)
                {
                    float t = i / (float)numberOfPoints;
                    Vector3 newPos =  Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
                    Debug.DrawLine(beginPos, newPos);
                    beginPos = newPos;
                }
            }
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// Parent duck that swims arround and do quack sounds
///</summary>
public class Duck : RoomObject
{
    [SerializeField]
    protected Animator animator;

    [SerializeField]
    private Room room;

    [SerializeField]
    private TrailRenderer trail;
    private float lifeTimeTrail = .5f;

    [SerializeField]
    private DuckSwimArea swimArea;
    [SerializeField]
    private DuckSwimArea bigSwimArea;
    private DuckSwimArea currentSwimArea;


    public DuckSwimArea SwimArea {
        get { return currentSwimArea;}
    }


    private Vector3 velocity;
    public Vector3 Velocity {
        get { return velocity;}
        set { velocity = value; }
    }

    private bool active = false;

    protected FSM duckBehaviour;
    private SwimState swimState;
    private StaringState staringState;

    public Duck() {
        largeScale = 4f;
    }
    protected override void Awake() {
        lifeTimeTrail = trail.time;
        currentSwimArea = swimArea;
        SetUpBehaviour();
    }

    protected virtual void SetUpBehaviour() {
        swimState = new SwimState() {
            _duck = this
            };
        staringState = new StaringState() {
            _duck = this
            };

        swimState.staringState = staringState;
        staringState.swimState = swimState;

        duckBehaviour = new FSM(swimState);
    }

    public virtual void Quack() {
        animator.SetTrigger("quack");
        AudioHandler.Instance.Play3DSound(SFXFiles.duck, transform, .5f, IsEnlarged ? .5f : 1f, false, true, 30f);
    }

    public override void OnRoomEnter()
    {
        active = true;
        if (swimState != null) {
            swimState._player = room.Player.transform;
            staringState._player = room.Player.transform;
        }
        base.OnRoomEnter();
    }

    public override void OnRoomLeave()
    {
        active = false;
        base.OnRoomLeave();
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTrailLifeTime;
        TimeProperty.onTimeAppearing += UpdateTrailLifeTime;
        Water.OnWaterBigStart += SetBigSwimArea;
        Water.OnWaterNormal += SetNormalSwimArea;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing += UpdateTrailLifeTime;
        TimeProperty.onTimeAppearing += UpdateTrailLifeTime;
        Water.OnWaterBigStart -= SetBigSwimArea;
        Water.OnWaterNormal -= SetNormalSwimArea;
    }

    private void SetBigSwimArea() {
        if (!InSpace) return;
        currentSwimArea = bigSwimArea;
        Vector3 temp = transform.position;
        temp.y = currentSwimArea.transform.position.y;
        transform.position = temp;
    }
    private void SetNormalSwimArea() {
        if (!InSpace) return;

        currentSwimArea = swimArea;
        Vector3 temp = transform.position;
        temp.y = currentSwimArea.transform.position.y;
        temp.x = currentSwimArea.transform.position.x;
        temp.z = currentSwimArea.transform.position.z;
        transform.position = temp;

    }

    private void UpdateTrailLifeTime() {
        if (Room.TimeScale <= 0) return;
        if (trail != null) trail.time = lifeTimeTrail * (1/ Room.TimeScale);
    }

    private void Update() {
        if (Time.deltaTime == 0 ||  Room.TimeScale == 0 || !active) return;
        
        duckBehaviour.Update();
    }
    private void Reset() {
        Word = "duck";
        AlternativeWords = new string[] { "bird", "ducks", "life", "birds", "animal", "animals" };
    }

}

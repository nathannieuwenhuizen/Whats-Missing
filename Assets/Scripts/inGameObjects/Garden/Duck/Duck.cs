using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// Parent duck that swims arround and do quack sounds
///</summary>
public class Duck : RoomObject
{
    [SerializeField]
    private Room room;

    [SerializeField]
    private DuckSwimArea swimArea;
    public DuckSwimArea SwimArea {
        get { return swimArea;}
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
    private void Awake() {
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
        AudioHandler.Instance.Player3DSound(SFXFiles.duck, transform, .5f, IsEnlarged ? .5f : 1f, false, true, 30f);
    }

    public override void OnRoomEnter()
    {
        active = true;
        Debug.Log("room = " + room);
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

    private void Update() {
        if (Time.deltaTime == 0 ||  Room.TimeScale == 0 || !active) return;
        
        duckBehaviour.Update();
    }
    private void Reset() {
        Word = "duck";
        AlternativeWords = new string[] { "bird", "ducks", "life", "birds", "animal", "animals" };
    }

}

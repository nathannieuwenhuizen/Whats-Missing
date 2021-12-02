using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : RoomObject
{
    [SerializeField]
    private Room room;

    [SerializeField]
    private DuckSwimArea swimArea;

    private bool active = false;

    private FSM duckBehaviour;
    private SwimState swimState;
    private StaringState staringState;
    private void Awake() {
        swimState = new SwimState() {
            _transform = transform, 
            _swimArea = swimArea,
            };
        staringState = new StaringState() {
            _transform = transform, 
            };

        swimState.staringState = staringState;
        staringState.swimState = swimState;

        duckBehaviour = new FSM(swimState);
    }

    public override void OnRoomEnter()
    {
        active = true;
        swimState._player = room.Player.transform;
        staringState._player = room.Player.transform;
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
